using DiscordBot.Caching;
using DiscordBot.Filters;
using DiscordBot.Pushshift;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Queries;

/// <summary>
/// A search query for NSFW content.
/// </summary>
[UsedImplicitly]
public class NsfwQueryHandler : QueryHandler
{
    public NsfwQueryHandler(
        QueryMultiplexer queryMultiplexer,
        ResultsCache cache,
        ResultProber resultsProber,
        ILogger<NsfwQueryHandler> logger) : base(queryMultiplexer, cache, resultsProber, logger) { }

    protected override PushshiftQuery ConfigureQuery(PushshiftQuery query)
        => query.Nsfw();
}