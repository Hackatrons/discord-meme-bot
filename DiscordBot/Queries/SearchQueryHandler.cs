using DiscordBot.Caching;
using DiscordBot.Filters;
using DiscordBot.Pushshift;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Queries;

/// <summary>
/// A search query for general content (can be both sfw and nsfw).
/// </summary>
[UsedImplicitly]
public class SearchQueryHandler : BaseQueryHandler
{
    public SearchQueryHandler(
        ResultsCache cache,
        ResultProber resultsProber,
        IHttpClientFactory httpClientFactory,
        ILogger<SearchQueryHandler> logger) : base(cache, resultsProber, httpClientFactory, logger) { }

    protected override PushshiftQuery BuildBaseQuery(string query) =>
        new PushshiftQuery()
            .Search(query);
}