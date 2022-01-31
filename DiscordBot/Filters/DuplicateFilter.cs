using DiscordBot.Language;
using DiscordBot.Models;

namespace DiscordBot.Filters;

/// <summary>
/// Removes duplicates from the result set based on the URL.
/// </summary>
public class DuplicateFilter : IResultFilter
{
    public IAsyncEnumerable<SearchResult> Filter(IAsyncEnumerable<SearchResult> input) => input
        .ThrowIfNull()
        .Distinct(x => x.Url);
}