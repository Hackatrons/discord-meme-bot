// ReSharper disable UnusedMember.Global

using System.Text.Json.Serialization;

namespace DiscordBot.Pushshift.Models;

public class PushshiftResult
{
    [JsonPropertyName("url")]
    public string? Url { get; set; }
    [JsonPropertyName("post_hint")]
    public string? PostHint { get; set; }
}