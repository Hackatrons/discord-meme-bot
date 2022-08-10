using System.Text.Json.Serialization;

namespace DiscordBot.Pushshift.Models;

public record RedditVideoPreview
{
    [JsonPropertyName("fallback_url")]
    public string? FallbackUrl { get; init; }
}