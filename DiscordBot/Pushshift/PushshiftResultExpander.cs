using DiscordBot.Pushshift.Models;
using DiscordBot.Text;

namespace DiscordBot.Pushshift;

/// <summary>
/// Extract additional URLs from pushshift results.
/// </summary>
internal static class PushshiftResultExpander
{
    /// <summary>
    /// Extracts any additional urls from pushshift results from metadata and the reddit submission text.
    /// </summary>
    public static IEnumerable<string> ExtractUrls(this PushshiftResult result)
    {
        var fromSelfText = !string.IsNullOrWhiteSpace(result.Selftext)
            ? UrlRegex.Match(result.Selftext)
            : Enumerable.Empty<string>();

        return fromSelfText;
    }
}