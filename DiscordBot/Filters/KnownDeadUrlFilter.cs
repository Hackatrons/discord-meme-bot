namespace DiscordBot.Filters;

/// <summary>
/// Disallows non-embeddable reddit videos.
/// </summary>
public static class KnownDeadUrlFilter
{
    static readonly HashSet<string> KnownDeadUrls = new(StringComparer.OrdinalIgnoreCase)
    {
        "https://i.imgur.com/removed.png"
    };

    public static bool IsAllowed(string url) => !KnownDeadUrls.Contains(url);
}