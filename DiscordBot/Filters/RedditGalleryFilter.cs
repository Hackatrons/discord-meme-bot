using DiscordBot.Models;

namespace DiscordBot.Filters;

/// <summary>
/// Disallows reddit gallery posts.
/// </summary>
public static class RedditGalleryFilter
{
    public static bool IsAllowed(SearchResult result)
    {
        // reddit galleries aren't embeddable
        return !result.IsGallery.GetValueOrDefault();
    }
}