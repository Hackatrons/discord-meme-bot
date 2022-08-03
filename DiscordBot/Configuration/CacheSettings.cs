using System.ComponentModel.DataAnnotations;

namespace DiscordBot.Configuration;

/// <summary>
/// Caching configuration settings.
/// </summary>
public record CacheSettings
{
    /// <summary>
    /// How long to keep items in the cache for.
    /// </summary>
    [Required]
    public TimeSpan? Duration { get; init; }
}
