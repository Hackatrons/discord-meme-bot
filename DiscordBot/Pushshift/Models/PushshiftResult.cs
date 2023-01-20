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

    /// <summary>
    /// Whether or not the url points to a reddit gallery (www.reddit.com/gallery/abc).
    /// </summary>
    [JsonPropertyName("is_gallery")]
    public bool? IsGallery { get; init; }

    /// <summary>
    /// Whether or not the url points to a reddit self post.
    /// </summary>
    [JsonPropertyName("is_self")]
    public bool? IsSelf { get; init; }

    [JsonPropertyName("preview")]
    public Preview? Preview { get; init; }
}