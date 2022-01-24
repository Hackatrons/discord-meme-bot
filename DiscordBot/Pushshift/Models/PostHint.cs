// ReSharper disable UnusedMember.Global

namespace DiscordBot.Pushshift.Models;

public static class PostHint
{
    public static readonly IReadOnlyCollection<string> Media = new []
    {
        Video, Image
    };

    public const string Video = "rich:video";
    public const string Link = "link";
    public const string Image = "image";
    public const string Self = "self";
}