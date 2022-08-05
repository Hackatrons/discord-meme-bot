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
        ResultsCache cache,
        ResultProber resultsProber,
        IHttpClientFactory httpClientFactory,
        ILogger<SfwQueryHandler> logger) : base(cache, resultsProber, httpClientFactory, logger) { }

    protected override PushshiftQuery BuildBaseQuery(string query) =>
        new PushshiftQuery()
            .Search(query)
            .Sfw();
}