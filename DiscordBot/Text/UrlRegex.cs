using System.Text.RegularExpressions;
using DiscordBot.Language;

namespace DiscordBot.Text;

public static class UrlRegex
{
    // https://stackoverflow.com/questions/10576686/c-sharp-regex-pattern-to-extract-urls-from-given-string-not-full-html-urls-but
    static readonly Regex UrlMatchRegex = new(
        @"(http|https)://([\w_-]+(?:(?:\.[\w_-]+)+))([\w.,@?^=%&:/~+#-]*[\w@?^=%&/~+#-])?",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static IEnumerable<string> Match(string text)
    {
        text.ThrowIfNull();

        return UrlMatchRegex
            .Matches(text)
            .Select(x => x.Value);
    }
}