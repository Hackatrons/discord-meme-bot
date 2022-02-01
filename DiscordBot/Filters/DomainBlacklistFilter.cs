using DiscordBot.Language;
using DiscordBot.Models;
using DiscordBot.Text;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Filters;

/// <summary>
/// Filters out results that match a set of blacklisted domains.
/// </summary>
public class DomainBlacklistFilter : IResultFilter
{
    // TODO: move to config
    static readonly string[] BlacklistDomains =
    {
        // can't embed links from v.reddit.it
        // TODO: is there a service out there we can leverage that converts v.reddit links to embeddable links?
        "v.redd.it",
        // not media
        "reddit.com"
    };

    readonly ILogger<DomainBlacklistFilter> _logger;

    public DomainBlacklistFilter(ILogger<DomainBlacklistFilter> logger)
    {
        _logger = logger.ThrowIfNull();
    }

    static bool IsBlacklisted(string url) =>
        BlacklistDomains.Any(url.ContainsIgnoreCase);

    public IAsyncEnumerable<SearchResult> Filter(IAsyncEnumerable<SearchResult> input) => input
        .ThrowIfNull()
        .Where(x =>
        {
            var blacklisted = IsBlacklisted(x.Url);
            if (blacklisted)
                _logger.LogDebug("Excluding result {url} as the domain has been blacklisted.", x.Url);

            return !blacklisted;
        });
}