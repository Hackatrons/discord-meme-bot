using DiscordBot.Language;
using DiscordBot.Models;
using DiscordBot.Text;

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

    static bool IsBlacklisted(string url) =>
        BlacklistDomains.Any(url.ContainsIgnoreCase);

    public IAsyncEnumerable<SearchResult> Filter(IAsyncEnumerable<SearchResult> input) => input
        .ThrowIfNull()
        .Where(x => !IsBlacklisted(x.Url));
}