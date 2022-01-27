using DiscordBot.Language;
using DiscordBot.Pushshift.Models;
using DiscordBot.Time;

namespace DiscordBot.Models;

public record SearchResult
{
    public string Url { get; init; } = "";
    public MediaType? MediaHint { get; init; }
    public int NumberOfComments { get; init; }
    public DateTime CreatedUtc { get; init; }
    public int Score { get; init; }

    public static SearchResult FromPushshift(PushshiftResult result) => new()
    {
        Url = result.Url!.ThrowIfNullOrWhitespace(),
        MediaHint = result.PostHint switch
        {
            PostHint.Image => MediaType.Image,
            PostHint.RichVideo or PostHint.HostedVideo => MediaType.Video,
            _ => null
        },
        CreatedUtc = DateTimeExtensions.UnixTimeStampToDateTime(result.CreatedUtc.ThrowIfNull().Value),
        NumberOfComments = result.NumComments ?? 0,
        Score = result.Score ?? 0
    };
}