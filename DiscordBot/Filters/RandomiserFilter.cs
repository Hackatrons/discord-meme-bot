using DiscordBot.Collections;
using DiscordBot.Language;
using DiscordBot.Models;

namespace DiscordBot.Filters;

/// <summary>
/// Randomises the input result set.
/// </summary>
public class RandomiserFilter : IResultFilter
{
    public IAsyncEnumerable<SearchResult> Filter(IAsyncEnumerable<SearchResult> input) => input
        .ThrowIfNull()
        .Shuffle();
}