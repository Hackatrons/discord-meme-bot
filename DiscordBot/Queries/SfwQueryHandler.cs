using DiscordBot.Caching;
using DiscordBot.Filters;
using DiscordBot.Pushshift;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Queries;

/// <summary>
/// A search query for SFW content.
/// </summary>
[UsedImplicitly]
public class SfwQueryHandler : QueryHandler
{
    public SfwQueryHandler(
        QueryMultiplexer queryMultiplexer,
        ResultsCache cache,
        ResultProber resultsProber,
        ILogger<SfwQueryHandler> logger) : base(queryMultiplexer, cache, resultsProber, logger) { }

    protected override PushshiftQuery ConfigureQuery(PushshiftQuery query)
        => query.Sfw();
}