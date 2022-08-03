using DiscordBot.Text;

namespace DiscordBot.Filters;

/// <summary>
/// Filters out results that match a set of blacklisted domains.
/// </summary>
public static class DomainBlacklistFilter
{
    const string RedditVideoHost = "v.redd.it";

    static readonly string[] BlacklistDomains =
    {
        // not media
        "reddit.com"
    };

    public static bool IsAllowed(string url)
    {
        if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri))
            return false;

        if (!url.ContainsIgnoreCase(RedditVideoHost))
            return !BlacklistDomains.Any(url.ContainsIgnoreCase);

        // can't embed v.redd.it links
        // however if we've been provided with a direct link to a dash video or audio file
        // then allow it through
        // e.g.:
        // allow https://v.redd.it/123/DASH_720.mp4
        // allow https://v.redd.it/123/DASH_1_2_M
        // don't allow https://v.redd.it/123

        // this is a bit dodgy, could do with a better method
        // basically /123/DASH_720.mp4 but trimmed to 123/DASH_720.mp4 = 1 slash
        // /123 but trimmed to 123 = 0 slashes
        // so we want 1 or more slashes
        var numberOfSlashes = uri.PathAndQuery.Trim('/').Count(c => c == '/');
        return numberOfSlashes >= 1;
    }
}