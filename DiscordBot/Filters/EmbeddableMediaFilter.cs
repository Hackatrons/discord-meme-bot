using DiscordBot.Models;
using DiscordBot.Text;

namespace DiscordBot.Filters;

/// <summary>
/// Excludes non-embeddable media links from the result set.
/// Embeddable links are things such as videos, gifs, images, and audio.
/// Other content such as HTML links will be excluded.
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

    public static bool ProbablyEmbeddableMedia(SearchResult result) =>
        result.MediaHint is MediaType.Video or MediaType.Audio or MediaType.Audio ||
        ProbablyEmbeddableMedia(result.FinalUrl ?? throw new InvalidOperationException("result url cannot be null"));

    static bool ProbablyEmbeddableMedia(string url) =>
        IsMediaFile(url) ||
        MediaHostingDomains.Any(url.ContainsIgnoreCase);

    static bool IsMediaFile(string url) =>
        Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
        MimeTypes.TryGetMimeType(uri.LocalPath, out var mimeType) &&
        EmbeddableMimeTypes.Any(mimeType.ContainsIgnoreCase);
}