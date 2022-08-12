using System.Text.Json.Serialization;

namespace DiscordBot.Pushshift.Models;

/// <summary>
/// A pushshift json result.
/// </summary>
public record PushshiftResult
{
    /// <summary>
    /// Url of the result.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; init; }

    /// <summary>
    /// A hint of the type of media at the target URL.
    /// </summary>
    [JsonPropertyName("post_hint")]
    public string? PostHint { get; init; }

    [JsonPropertyName("preview")]
    public Preview? Preview { get; init; }
}