// ReSharper disable UnusedMember.Global

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
    /// True if the URL is a reddit text post, otherwise false if it's an external link.
    /// e.g.
    /// - For http://reddit.com/r/something/123 IsSelf will be true
    /// - For http://something.com is, IsSelf will be false
    /// </summary>
    [JsonPropertyName("is_self")]
    public bool? IsSelf { get; set; }
    /// <summary>
    /// Text of the reddit submission.
    /// </summary>
    [JsonPropertyName("selftext")]
    public string? Selftext { get; set; }
}