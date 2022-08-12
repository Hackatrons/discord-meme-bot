using DiscordBot.Language;
using DiscordBot.Models;
using DiscordBot.Pushshift;
using DiscordBot.Pushshift.Models;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Queries;

/// <summary>
/// Sends multiple queries to Pushshift and consolidates the results into a single result set.
/// </summary>
public class QueryMultiplexer
{
    // pushshift has a hard limit of 100
    const int SearchResultLimit = 100;

    readonly ILogger _logger;
    readonly IHttpClientFactory _httpClientFactory;

    public QueryMultiplexer(IHttpClientFactory httpClientFactory, ILogger<QueryMultiplexer> logger)
    {
        _httpClientFactory = httpClientFactory.ThrowIfNull();
        _logger = logger.ThrowIfNull();
    }

    /// <summary>
    /// Returns a set of search results for the given search criteria.
    /// </summary>
    /// <param name="searchText">The search term.</param>
    /// <param name="configureQuery">A callback function to configure any additional filtering/settings for each spawned query.</param>
    public async Task<IEnumerable<SearchResult>> GetResults(string searchText, Func<PushshiftQuery, PushshiftQuery> configureQuery)
    {
        using var httpClient = _httpClientFactory.CreateClient();

        // mix in the following search results in together:
        // 1. by highest score
        // 2. by most recent
        var mediaTasks = new[]
        {
            new PushshiftQuery()
                .Search(searchText)
                .PostHints(PostHint.Image, PostHint.RichVideo, PostHint.HostedVideo)
                .Sort(SortType.CreatedDate, SortDirection.Descending),

            new PushshiftQuery()
                .Search(searchText)
                .PostHints(PostHint.Image, PostHint.RichVideo, PostHint.HostedVideo)
                .Sort(SortType.Score, SortDirection.Descending)
        }
        .Select(x => x
            .Limit(SearchResultLimit)
            .Fields<PushshiftResult>())
        .Select(configureQuery)
        .Select(async query => new
        {
            query,
            // ReSharper disable once AccessToDisposedClosure
            results = (await query.Execute(httpClient)).ToList()
        });

        var mediaResults = (await Task.WhenAll(mediaTasks)).ToList();
        foreach (var task in mediaResults)
        {
            _logger.LogDebug("Found {count} results from url '{query}'.", task.results.Count, task.query.ToString());
        }

        var additionalResults = mediaResults
            .SelectMany(x => x.results)
            .SelectMany(x => x.ExtractUrls())
            .ToList();

        if (additionalResults.Any())
        {
            _logger.LogDebug("Extracted {count} additional results from metadata for query '{query}'.", additionalResults.Count, searchText);
        }

        return mediaResults
            .SelectMany(x => x.results)
            .DistinctBy(x => x.Url)
            .Select(SearchResult.FromPushshift)
            .UnionBy(additionalResults, x => x.Url);
    }
}