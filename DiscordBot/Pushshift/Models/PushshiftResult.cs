// ReSharper disable UnusedMember.Global

using System.Text.Json.Serialization;

namespace DiscordBot.Pushshift.Models;

/// <summary>
/// A pushshift json result.
/// </summary>
public record PushshiftResult
{
    [JsonPropertyName("url")]
    public string? Url { get; init; }
    [JsonPropertyName("post_hint")]
    public string? PostHint { get; init; }
    [JsonPropertyName("num_comments")]
    public int? NumComments { get; init; }
    [JsonPropertyName("created_utc")]
    public long? CreatedUtc { get; init; }
    [JsonPropertyName("score")]
    public int? Score { get; set; }
}