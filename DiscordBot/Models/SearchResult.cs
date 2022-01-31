using DiscordBot.Language;
using DiscordBot.Pushshift.Models;
using DiscordBot.Time;

namespace DiscordBot.Models;

/// <summary>
/// A business domain search result containing metadata about the result.
/// </summary>
public record SearchResult
{
    /// <summary>
    /// URL of the result.
    /// </summary>
    public string Url { get; init; } = "";
    /// <summary>
    /// An optional hint to the type of media of this result.
    /// </summary>
    public MediaType? MediaHint { get; init; }
    /// <summary>
    /// UTC datetime creation date.
    /// </summary>
    public DateTime CreatedUtc { get; init; }

    /// <summary>
    /// Constructs a search result from the given pushshift search result.
    /// </summary>
    public static SearchResult FromPushshift(PushshiftResult result) => new()
    {
        Url = result.Url!.ThrowIfNullOrWhitespace(),
        MediaHint = result.PostHint switch
        {
            PostHint.Image => MediaType.Image,
            PostHint.RichVideo or PostHint.HostedVideo => MediaType.Video,
            _ => null
        },
        CreatedUtc = DateTimeExtensions.UnixTimeStampToDateTime(result.CreatedUtc.ThrowIfNull().Value)
    };
}