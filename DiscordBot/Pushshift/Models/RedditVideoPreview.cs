// ReSharper disable UnusedMember.Global
namespace DiscordBot.Pushshift.Models;

public class RedditVideoPreview
{
    public string? DashUrl { get; set; }
    public long? Duration { get; set; }
    public string? FallbackUrl { get; set; }
    public long? Height { get; set; }
    public string? HlsUrl { get; set; }
    public bool? IsGif { get; set; }
    public string? ScrubberMediaUrl { get; set; }
    public string? TranscodingStatus { get; set; }
    public long? Width { get; set; }
}