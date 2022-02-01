namespace DiscordBot.Pushshift.Models;

/// <summary>
/// A pushshift post hint.
/// </summary>
public static class PostHint
{
    /// <summary>
    /// External video links.
    /// </summary>
    public const string RichVideo = "rich:video";
    /// <summary>
    /// Videos that are hosted by reddit.
    /// </summary>
    public const string HostedVideo = "hosted:video";
    public const string Image = "image";
}