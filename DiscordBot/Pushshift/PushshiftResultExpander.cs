using DiscordBot.Models;
using DiscordBot.Pushshift.Models;

namespace DiscordBot.Pushshift;

/// <summary>
/// Extract additional URLs from pushshift results.
/// </summary>
internal static class PushshiftResultExpander
{
    /// <summary>
    /// Extracts any additional urls from pushshift results from metadata fields.
    /// </summary>
    public static IEnumerable<SearchResult> ExtractUrls(this PushshiftResult result)
    {
        // the dash format uses two separate files; on for video and one for audio
        // there is no combined version with both audio and video unfortunately
        // so best we can do is provide the video
        var videoPreview = result.Preview?.RedditVideoPreview?.FallbackUrl;

        if (videoPreview != null)
            yield return new SearchResult { Url = videoPreview, MediaHint = MediaType.Video };
    }
}