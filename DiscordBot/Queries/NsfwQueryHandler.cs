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
public class NsfwQueryHandler : BaseQueryHandler
{
    public NsfwQueryHandler(
        ResultsCache cache,
        ResultProber resultsProber,
        IHttpClientFactory httpClientFactory,
        ILogger<NsfwQueryHandler> logger) : base(cache, resultsProber, httpClientFactory, logger) { }

    protected override PushshiftQuery BuildBaseQuery(string query) =>
        new PushshiftQuery()
            .Search(query)
            .Nsfw();
}