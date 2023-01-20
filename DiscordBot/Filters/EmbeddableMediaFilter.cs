using DiscordBot.Models;
using DiscordBot.Text;

namespace DiscordBot.Filters;

/// <summary>
/// Determines if a search result is likely to be embeddable.
/// Embeddable links are things such as videos, gifs, images, and audio.
/// Other content such as general HTML links will be excluded.
/// The embeddable media detection method is not an exact science and may result in false positives and negatives.
/// </summary>
public static class EmbeddableMediaFilter
{
    static readonly string[] MediaHostingDomains =
    {
        "gfycat.com",
        "giphy.com",
        "imgur.com",
        "i.redditmedia.com",
        "instagram.com",
        "streamable.com",
        "youtube.com",
        "youtu.be",
        "v.redd.it"
    };

    static readonly string[] EmbeddableMimeTypes =
    {
        "audio",
        "image",
        "video"
    };

    /// <summary>
    /// Checks the url, filename, and probe content-type result to determine if the search result is embeddable.
    /// </summary>
    public static bool ProbablyEmbeddableMedia(SearchResult result)
    {
        _ = result.FinalUrl ?? throw new InvalidOperationException("result url cannot be null");

        // trust it's embeddable if specified by the api
        if (result.MediaHint is MediaType.Video or MediaType.Audio or MediaType.Image)
            return true;

        // trust it's embeddable if it's from a known media site
        if (MediaHostingDomains.Any(result.FinalUrl.ContainsIgnoreCase))
            return true;

        // if it has a filename in the url like something.png that maps to a media content type, we'll trust that it's embeddable
        if (Uri.TryCreate(result.FinalUrl, UriKind.Absolute, out var uri) &&
            MimeTypes.TryGetMimeType(uri.LocalPath, out var mimeType) &&
            EmbeddableMimeTypes.Any(mimeType.ContainsIgnoreCase))
            return true;

        // check the content-type specified by the http response
        return result.Probe?.ContentType != null && EmbeddableMimeTypes.Any(result.Probe.ContentType.ContainsIgnoreCase);
    }
}