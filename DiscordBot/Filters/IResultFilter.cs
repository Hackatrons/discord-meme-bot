using DiscordBot.Models;

namespace DiscordBot.Filters;

internal interface IResultsFilter
{
    IAsyncEnumerable<SearchResult> Filter(IAsyncEnumerable<SearchResult> input);
}