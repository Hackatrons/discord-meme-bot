using DiscordBot.Models;

namespace DiscordBot.Filters;

/// <summary>
/// Disallows reddit self posts (e.g. www.reddit.com/r/something/somepost
/// </summary>
public static class RedditSelfPostFilter
{
    public static bool IsAllowed(SearchResult result)
    {
        // reddit self posts aren't embeddable
        return !result.IsSelf.GetValueOrDefault();
    }
}