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
public class SearchQueryHandler : QueryHandler
{
    public SearchQueryHandler(
        QueryMultiplexer queryMultiplexer,
        ResultsCache cache,
        ResultProber resultsProber,
        ILogger<SearchQueryHandler> logger) : base(queryMultiplexer, cache, resultsProber, logger) { }

    protected override PushshiftQuery ConfigureQuery(PushshiftQuery query)
        => query;
}