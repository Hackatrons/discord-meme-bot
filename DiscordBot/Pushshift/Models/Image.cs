// ReSharper disable UnusedMember.Global
namespace DiscordBot.Pushshift.Models;

public class Image
{
    public string? Id { get; set; }
    public Source[]? Resolutions { get; set; }
    public Source? Source { get; set; }
    public Variants? Variants { get; set; }
}