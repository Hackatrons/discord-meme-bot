// ReSharper disable UnusedMember.Global

using System.Text.Json.Serialization;

namespace DiscordBot.Pushshift.Models;

public record Preview
{
    [JsonPropertyName("enabled")]
    public bool? Enabled { get; init; }

    [JsonPropertyName("reddit_video_preview")]
    public RedditVideoPreview? RedditVideoPreview { get; init; }
}