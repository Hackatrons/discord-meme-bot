// ReSharper disable UnusedMember.Global

using System.Text.Json.Serialization;

namespace DiscordBot.Pushshift.Models;

public class Preview
{
    [JsonPropertyName("enabled")]
    public bool? Enabled { get; set; }

    [JsonPropertyName("reddit_video_preview")]
    public RedditVideoPreview? RedditVideoPreview { get; set; }
}