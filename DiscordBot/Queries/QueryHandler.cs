using DiscordBot.Caching;
using DiscordBot.Collections;
using DiscordBot.Filters;
using DiscordBot.Language;
using DiscordBot.Models;
using DiscordBot.Pushshift;
using DiscordBot.Pushshift.Models;
using DiscordBot.Threading;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Queries;

/// <summary>
/// Base class for search queries.
/// </summary>
public abstract class QueryHandler
{
    // pushshift has a hard limit of 100
    const int SearchResultLimit = 100;

    readonly ResultsCache _cache;
    readonly IHttpClientFactory _httpClientFactory;
    readonly string _typeName;
    readonly ILogger _logger;
    readonly ResultProber _resultProber;

    protected QueryHandler(
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
        var results = await _cache.Get(channelId, _typeName, query);

        if (results == null || !results.Any())
        {
            results = (await GetResults(query))
                // filter out unwanted results
                .Where(ResultAllowed)
                // randomise the result set
                .Shuffle()
                .ToList();

            // store the results in the cache
            await _cache.Set(channelId, _typeName, query, results);
        }

        // list of results that have already been used
        var consumed = results.Where(x => x.Consumed).ToList();
        var remaining = new Stack<SearchResult>(results
            .Except(consumed)
            // put pre-primed results at the top
            .OrderBy(x => x.Probe != null));

        SearchResult? nextResult;
        var foundResult = false;

        while (remaining.TryPop(out nextResult))
        {
            // flag the result as used
            nextResult.Consumed = true;

            if (nextResult.Probe != null)
                _logger.LogDebug("Using primed result: '{url}'", nextResult.FinalUrl);

            // probe if we haven't already
            nextResult.Probe ??= await _resultProber.Probe(nextResult);

            if (!ResultAllowed(nextResult))
                continue;

            // check for duplicates based on the redirected url and etag
            var previous = consumed.FirstOrDefault(x =>
                string.Equals(x.FinalUrl, nextResult.FinalUrl, StringComparison.OrdinalIgnoreCase) ||
                x.Probe?.Etag == nextResult.Probe?.Etag);

            if (previous != null)
            {
                _logger.LogDebug(
                    "Excluding result '{duplicateUrl}' as it's a duplicate of '{url}'." +
                    "Previous result ETag : '{previousResultRedirectUrl}', " +
                    "Duplicate result ETag : '{duplicateResultRedirectUrl}'.",
                    nextResult.FinalUrl,
                    previous.FinalUrl,
                    previous.Probe?.Etag,
                    nextResult.Probe?.Etag);

                continue;
            }

            // we have found a result to use
            foundResult = true;
            break;
        }

        // update the cache with the consumed flags and probe results
        await _cache.Set(channelId, _typeName, query, results);

        // pre-prime some results to speed up delivery
        PrimeResults(results, channelId, query).Forget();

        return (foundResult ? nextResult : null, results.Count > 0 && !foundResult);
    }

    /// <summary>
    /// Builds a base pushshift query for the given search string.
    /// </summary>
    protected abstract PushshiftQuery BuildBaseQuery(string query);

    async Task PrimeResults(IReadOnlyCollection<SearchResult> results, ulong channelId, string query)
    {
        // don't prime results if there are already X primed
        const int minPrimedResults = 2;
        // prime the next X results
        const int numberResultsToPrime = 5;

        var primedResultsCount = results.Count(x => !x.Consumed && x.Probe != null);
        if (primedResultsCount >= minPrimedResults)
            return;

        // probe the next couple of results ahead of time
        // to reduce the time it takes to return a result
        var resultsToPrime = results
            .Where(x => !x.Consumed && x.Probe == null)
            .Take(numberResultsToPrime);

        await Task.WhenAll(resultsToPrime
            .Select(async x =>
            {
                _logger.LogDebug("Priming result: '{url}'", x.Url);
                x.Probe = await _resultProber.Probe(x);
                return x;
            }));

        // update the cache with the consumed flags and probe results
        await _cache.Set(channelId, _typeName, query, results);
    }

    bool ResultAllowed(SearchResult result)
    {
        if (!DomainBlacklistFilter.IsAllowed(result.FinalUrl))
        {
            _logger.LogDebug("Excluding result '{url}' as the domain has been blacklisted.", result.FinalUrl);
            return false;
        }

        if (!EmbeddableMediaFilter.ProbablyEmbeddableMedia(result))
        {
            _logger.LogDebug("Excluding result '{url}' as the url has been deemed as likely non-embeddable.", result.FinalUrl);
            return false;
        }

        // ReSharper disable once InvertIf
        if (result.Probe is { IsAlive: false })
        {
            _logger.LogDebug(
                "Excluding result '{url}' as the url probe determined the url is dead. " +
                "HTTP Status Code: '{statusCode}', " +
                "Error: '{error}'.",
                result.FinalUrl,
                result.Probe.HttpStatusCode?.ToString() ?? "(none)",
                result.Probe.Error ?? "(none)");

            return false;
        }

        return true;
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