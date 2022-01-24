// ReSharper disable UnusedMember.Global
namespace DiscordBot.Pushshift.Models;

public class Preview
{
    public bool? Enabled { get; set; }
    public Image[]? Images { get; set; }
    public RedditVideoPreview? RedditVideoPreview { get; set; }
}