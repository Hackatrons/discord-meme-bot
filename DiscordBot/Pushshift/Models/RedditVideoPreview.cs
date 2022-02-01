using System.Text.Json.Serialization;

namespace DiscordBot.Pushshift.Models;

public class RedditVideoPreview
{
    [JsonPropertyName("fallback_url")]
    public string? FallbackUrl { get; set; }
}