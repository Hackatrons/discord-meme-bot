using DiscordBot.Caching;
using DiscordBot.Collections;
using DiscordBot.Filters;
using DiscordBot.Language;
using DiscordBot.Models;
using DiscordBot.Pushshift;
using DiscordBot.Threading;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Queries;

/// <summary>
/// Orchestrates sendings queries to Pushshift, filtering, caching, and priming.
/// </summary>
public abstract class QueryHandler
{
    readonly QueryMultiplexer _queryMultiplexer;
    readonly ResultsCache _cache;
    readonly string _typeName;
    readonly ILogger _logger;
    readonly ResultProber _resultProber;

    protected QueryHandler(
        QueryMultiplexer queryMultiplexer,
        ResultsCache cache,
        ResultProber resultProber,
        ILogger logger)
    {
        _queryMultiplexer = queryMultiplexer.ThrowIfNull();
        _resultProber = resultProber.ThrowIfNull();
        _cache = cache.ThrowIfNull();
        _typeName = GetType().Name;
        _logger = logger.ThrowIfNull();
    }

    /// <summary>
    /// Retrieves the next search result for the given query.
    /// Caches search results based on the channel and query.
    /// </summary>
    public async Task<(SearchResult? Result, bool Finished)> SearchNext(string query, ulong channelId)
    {
        var results = await GetResults(query, channelId);

        // list of results that have already been used
        var consumed = results.Where(x => x.Consumed).ToList();
        var remaining = new Stack<SearchResult>(results
            .Except(consumed)
            // randomise the result set
            .Shuffle()
            // prioritise pre-primed results
            // (note this is possible after randomisation because OrderBy is a stable sort)
            .OrderBy(x => x.Probe != null)
            // prioritise embeddables
            .ThenBy(EmbeddableMediaFilter.ProbablyEmbeddableMedia));

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

            if (!ResultAllowed(nextResult) || IsDuplicate(nextResult, consumed))
                continue;

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
    /// Configures the pushshift query with any filters/settings for this query type.
    /// </summary>
    protected abstract PushshiftQuery ConfigureQuery(PushshiftQuery query);

    async Task<IReadOnlyCollection<SearchResult>> GetResults(string query, ulong channelId)
    {
        var results = await _cache.Get(channelId, _typeName, query);

        if (results != null && results.Any())
            return results;

        var unfiltered = (await _queryMultiplexer.GetResults(query, ConfigureQuery)).ToList();

        _logger.LogInformation("Found {count} results for query '{query}'.", unfiltered.Count, query);

        var filtered = unfiltered
            // filter out unwanted results
            .Where(ResultAllowed)
            .ToList();

        _logger.LogInformation("{remaining}/{total} results remain after filtering for query '{query}'.", filtered.Count, unfiltered.Count, query);

        // store the results in the cache
        await _cache.Set(channelId, _typeName, query, filtered);

        return filtered;
    }

    async Task PrimeResults(IReadOnlyCollection<SearchResult> results, ulong channelId, string query)
    {
        // don't prime results if there are already X primed
        const int minPrimedResults = 2;
        // prime the next X results
        const int numberResultsToPrime = 5;

        var primedResultsCount = results.Count(x => x is { Consumed: false, Probe: { } });
        if (primedResultsCount >= minPrimedResults)
            return;

        // probe the next couple of results ahead of time
        // to reduce the time it takes to return a result
        var resultsToPrime = results
            .Where(x => x is { Consumed: false, Probe: null })
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
        if (!KnownDeadUrlFilter.IsAllowed(result.FinalUrl))
        {
            _logger.LogDebug("Excluding result '{url}' as it's a known dead url.", result.FinalUrl);
            return false;
        }

        if (!RedditVideoDomainFilter.IsAllowed(result.FinalUrl))
        {
            _logger.LogDebug("Excluding result '{url}' as it's a reddit video that's not embeddable.", result.FinalUrl);
            return false;
        }

        if (!RedditGalleryFilter.IsAllowed(result))
        {
            _logger.LogDebug("Excluding result '{url}' as it's a reddit gallery that's not embeddable.", result.FinalUrl);
            return false;
        }

        if (!RedditSelfPostFilter.IsAllowed(result))
        {
            _logger.LogDebug("Excluding result '{url}' as it's a reddit self post that's not embeddable.", result.FinalUrl);
            return false;
        }

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

        // ReSharper disable once InvertIf
        if (result.Probe?.ContentType != null && !EmbeddableMediaFilter.ProbablyEmbeddableMedia(result))
        {
            // if we've probed the url and it's still not embeddable, don't allow it
            _logger.LogDebug("Excluding result '{url}' as the url probe determined the url is not embeddable media.", result.FinalUrl);

            return false;
        }

        return true;
    }

    bool IsDuplicate(SearchResult result, IEnumerable<SearchResult> previouslyConsumed)
    {
        // check for duplicates based on the redirected url and etag
        var previous = previouslyConsumed.FirstOrDefault(x =>
            string.Equals(x.FinalUrl, result.FinalUrl, StringComparison.OrdinalIgnoreCase) ||
            (x.Probe?.Etag != null && result.Probe?.Etag != null && string.Equals(x.Probe.Etag, result.Probe.Etag, StringComparison.OrdinalIgnoreCase)));

        if (previous == null) return false;

        _logger.LogDebug(
            "Excluding result '{duplicateUrl}' as it's a duplicate of '{url}'." +
            "Previous result ETag : '{previousResultRedirectUrl}', " +
            "Duplicate result ETag : '{duplicateResultRedirectUrl}'.",
            result.FinalUrl,
            previous.FinalUrl,
            previous.Probe?.Etag,
            result.Probe?.Etag);

        return true;
    }
}