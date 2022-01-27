using DiscordBot.Language;
using DiscordBot.Models;
using DiscordBot.Text;

namespace DiscordBot.Filters;

public class EmbeddableMediaFilter : IResultsFilter
{
    // TODO: move to config
    static readonly string[] MediaHostingDomains =
    {
        "gfycat.com",
        "giphy.com",
        "imgur.com",
        "i.redditmedia.com",
        "instagram.com",
        "streamable.com",
        "youtube.com",
        "youtu.be"
    };

    static readonly string[] EmbeddableMimeTypes =
    {
        "audio",
        "image",
        "video"
    };

    public IAsyncEnumerable<SearchResult> Filter(IAsyncEnumerable<SearchResult> input) => input
        .ThrowIfNull()
        .Where(ProbablyEmbeddableMedia);

    static bool ProbablyEmbeddableMedia(SearchResult result) =>
        result.MediaHint is MediaType.Video or MediaType.Audio or MediaType.Audio ||
        ProbablyEmbeddableMedia(result.Url ?? throw new InvalidOperationException("result url cannot be null"));

    static bool ProbablyEmbeddableMedia(string url) =>
        IsMediaFile(url) ||
        MediaHostingDomains.Any(url.ContainsIgnoreCase);

    static bool IsMediaFile(string url) =>
        Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
        MimeTypes.TryGetMimeType(uri.LocalPath, out var mimeType) &&
        EmbeddableMimeTypes.Any(mimeType.ContainsIgnoreCase);
}