using DiscordBot.Models;
using DiscordBot.Pushshift.Models;
using DiscordBot.Text;

namespace DiscordBot.Pushshift;

/// <summary>
/// Extract additional URLs from pushshift results.
/// </summary>
internal static class PushshiftResultExpander
{
    /// <summary>
    /// Extracts any additional urls from pushshift results from metadata fields and the reddit submission text.
    /// </summary>
    public static IEnumerable<SearchResult> ExtractUrls(this PushshiftResult result)
    {
        // note we can't use the image previews (result.preview.images), as reddit blocks them unless the referrer comes from reddit I think
        var fromSelfText = !string.IsNullOrWhiteSpace(result.Selftext)
            ? UrlRegex.Match(result.Selftext)
            : Enumerable.Empty<string>();

        // the dash format uses two separate files; on for video and one for audio
        // there is no combined version with both audio and video unfortunately
        // so best we can do is provide the video
        var videoPreview = result.Preview?.RedditVideoPreview?.FallbackUrl;

        foreach (var url in fromSelfText)
            yield return new SearchResult { Url = url };

        if (videoPreview != null)
            yield return new SearchResult { Url = videoPreview, MediaHint = MediaType.Video };
    }
}