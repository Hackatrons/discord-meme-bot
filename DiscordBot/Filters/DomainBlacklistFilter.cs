using DiscordBot.Language;
using DiscordBot.Models;

namespace DiscordBot.Filters;

public class DomainBlacklistFilter : IResultsFilter
{
    // TODO: move to config
    static readonly string[] BlacklistDomains =
    {
        // can't embed links from v.reddit.it
        // TODO: is there a service out there we can leverage that converts v.reddit links to embeddable links?
        "v.redd.it",
        // not media
        "reddit.com",
    };

    static bool IsBlacklisted(string url) => BlacklistDomains.Any(x => url.Contains(x, StringComparison.OrdinalIgnoreCase));

    public IAsyncEnumerable<SearchResult> Filter(IAsyncEnumerable<SearchResult> input) =>
        input.ThrowIfNull().Where(x => !IsBlacklisted(x.Url));
}