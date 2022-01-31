using DiscordBot.Models;

namespace DiscordBot.Filters;

/// <summary>
/// Exposes a method that filters a set of search results.
/// </summary>
internal interface IResultFilter
{
    /// <summary>
    /// Returns a new async-enumerable with the filter applied.
    /// </summary>
    IAsyncEnumerable<SearchResult> Filter(IAsyncEnumerable<SearchResult> input);
}