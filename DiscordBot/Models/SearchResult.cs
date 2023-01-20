using DiscordBot.Language;
using DiscordBot.Pushshift.Models;

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
    /// The probe's redirected url, otherwise the search result url.
    /// </summary>
    public string FinalUrl => Probe?.RedirectedUrl ?? Url;

    /// <summary>
    /// An optional hint to the type of media of this result.
    /// </summary>
    public MediaType? MediaHint { get; init; }

    /// <summary>
    /// True if this search result has been used (i.e. sent to the discord chat), otherwise false.
    /// </summary>
    public bool Consumed { get; set; }

    /// <summary>
    /// Contains the result of the URL probe (if this result has been probed).
    /// </summary>
    public ProbeResult? Probe { get; set; }

    /// <summary>
    /// Whether or not the url points to a reddit gallery (www.reddit.com/gallery/abc).
    /// </summary>
    public bool? IsGallery { get; init; }

    /// <summary>
    /// Whether or not the url points to a reddit text post.
    /// </summary>
    public bool? IsSelf { get; init; }

    /// <summary>
    /// Constructs a search result from the given pushshift search result.
    /// </summary>
    public static SearchResult FromPushshift(PushshiftResult result) => new()
    {
        Url = result.Url!.ThrowIfNullOrWhitespace(),
        IsGallery = result.IsGallery,
        IsSelf = result.IsSelf,
        MediaHint = result.PostHint switch
        {
            PostHint.Image => MediaType.Image,
            PostHint.RichVideo or PostHint.HostedVideo => MediaType.Video,
            _ => null
        }
    };
}