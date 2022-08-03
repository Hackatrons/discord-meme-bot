﻿using DiscordBot.Caching;
using DiscordBot.Collections;
using DiscordBot.Filters;
using DiscordBot.Language;
using DiscordBot.Models;
using DiscordBot.Pushshift;
using DiscordBot.Pushshift.Models;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Queries;

/// <summary>
/// Base class for search queries.
/// </summary>
public abstract class BaseQueryHandler
{
    // pushshift has a hard limit of 100
    const int SearchResultLimit = 100;

    readonly ResultsCache _cache;
    readonly IHttpClientFactory _httpClientFactory;
    readonly string _typeName;
    readonly ILogger _logger;
    readonly ResultProber _resultProber;

    protected BaseQueryHandler(
        ResultsCache cache,
        ResultProber resultProber,
        IHttpClientFactory httpClientFactory,
        ILogger logger)
    {
        _resultProber = resultProber.ThrowIfNull();
        _cache = cache.ThrowIfNull();
        _httpClientFactory = httpClientFactory.ThrowIfNull();
        _typeName = GetType().Name;
        _logger = logger.ThrowIfNull();
    }

    /// <summary>
    /// Retrieves the next search result for the given query.
    /// Caches search results based on the channel and query.
    /// </summary>
    public async Task<(SearchResult? Result, bool Finished)> SearchNext(string query, ulong channelId)
    {
        var results = await _cache.Get(
            channelId,
            _typeName,
            query);

        if (results == null || results.Count == 0)
        {
            var newResults = (await GetResults(query))
                // randomise the result set
                .Shuffle()
                .ToList();

            // filter out unwanted results
            results = FilterResults(newResults).ToList();

            // store the results in the cache
            await _cache.Set(channelId, _typeName, query, results);
        }

        SearchResult? nextResult;

        // list of results that have already been used
        var consumed = results.Where(x => x.Consumed).ToList();
        var remaining = new Stack<SearchResult>(results.Except(consumed));

        while (remaining.TryPop(out nextResult))
        {
            // flag the result as used
            nextResult.Consumed = true;

            // probe the result to determine if the url is alive
            // as we don't want to send dead/404 links to the user
            nextResult.Probe = await _resultProber.Probe(nextResult);
            if (!nextResult.Probe.Success)
            {
                _logger.LogDebug(
                    "Excluding result '{url}' as the url probe was unsuccessful. " +
                    "HTTP Status Code: '{statusCode}', " +
                    "Error: '{error}'.",
                    nextResult.Url,
                    nextResult.Probe.HttpStatusCode?.ToString() ?? "(none)",
                    nextResult.Probe.Error ?? "(none)");
            }

            // check for duplicates based on the redirected url and etag
            var previous = consumed.FirstOrDefault(x =>
                (x.Probe?.RedirectedUrl ?? x.Url) == (nextResult.Probe.RedirectedUrl ?? nextResult.Url) ||
                x.Probe?.Etag == nextResult.Probe?.Etag);

            if (previous != null)
            {
                _logger.LogDebug(
                    "Excluding result '{duplicateUrl}' as it's a duplicate of '{url}'." +
                    "Previous result redirect url: '{previousResultRedirectUrl}', " +
                    "Duplicate result redirect url: '{duplicateResultRedirectUrl}', " +
                    "Previous result ETag : '{previousResultRedirectUrl}', " +
                    "Duplicate result ETag : '{duplicateResultRedirectUrl}'.",
                    nextResult.Url,
                    previous.Url,
                    previous.Probe?.RedirectedUrl,
                    nextResult.Probe?.RedirectedUrl,
                    previous.Probe?.Etag,
                    nextResult.Probe?.Etag);
            }
            // we have found a result to use
            else break;
        }

        // update the cache with the consumed flags and probe results
        await _cache.Set(channelId, _typeName, query, results);

        return (nextResult, results.Count > 0 && nextResult == null);
    }

    /// <summary>
    /// Builds a base pushshift query for the given search string.
    /// </summary>
    protected abstract PushshiftQuery BuildBaseQuery(string query);

    IEnumerable<SearchResult> FilterResults(IEnumerable<SearchResult> results)
    {
        var filtered = results.Where(x =>
        {
            var allowed = DomainBlacklistFilter.IsAllowed(x.Url);
            if (!allowed)
                _logger.LogDebug("Excluding result {url} as the domain has been blacklisted.", x.Url);

            return allowed;
        });

        filtered = filtered.Where(x =>
        {
            var allowed = EmbeddableMediaFilter.ProbablyEmbeddableMedia(x);
            if (!allowed)
                _logger.LogDebug("Excluding result {url} as the url has been deemed as likely non-embeddable.", x.Url);

            return allowed;
        });

        return filtered;
    }

    async Task<IEnumerable<SearchResult>> GetResults(string query)
    {
        using var httpClient = _httpClientFactory.CreateClient();

        var baseQuery = BuildBaseQuery(query);

        // mix 2 search results in together:
        // 1. ordered by highest score
        // 2. ordered by most recent
        // probably a good mixture of results
        var mostRecentTask = baseQuery
            .Clone()
            .Limit(SearchResultLimit)
            .Sort(SortType.CreatedDate, SortDirection.Descending)
            .Fields<PushshiftResult>()
            .Execute(httpClient);

        var highestScoreTask = baseQuery
            .Clone()
            .Limit(SearchResultLimit)
            .Sort(SortType.Score, SortDirection.Descending)
            .Fields<PushshiftResult>()
            .Execute(httpClient);

        await Task.WhenAll(mostRecentTask, highestScoreTask);

        var mostRecent = (await mostRecentTask).ToList();
        var highestScore = (await highestScoreTask).ToList();

        // merge the result sets
        var combined = mostRecent
            .UnionBy(highestScore, x => x.Url)
            // filter out reddit self posts
            .Where(x => !x.IsSelf.GetValueOrDefault());

        // extract any additional results from reddit posts
        var additionalResults = mostRecent
            .SelectMany(x => x.ExtractUrls())
            .UnionBy(highestScore.SelectMany(x => x.ExtractUrls()), x => x.Url)
            .ToList();

        // merge the additional results in
        // the final result be unique by url
        var results = combined
            .Select(SearchResult.FromPushshift)
            .UnionBy(additionalResults, x => x.Url)
            .ToList();

        _logger.LogDebug("Found {count} most recent results for query '{query}'.", mostRecent.Count, query);
        _logger.LogDebug("Found {count} highest score results for query '{query}'.", highestScore.Count, query);
        _logger.LogDebug("Found {count} additional urls for query '{query}'.", additionalResults.Count, query);
        _logger.LogDebug("After merging, a total of {count} results were found for query '{query}'.", results.Count, query);

        return results;
    }
}