using DiscordBot.Language;
using DiscordBot.Models;

namespace DiscordBot.Filters;

public class DuplicateFilter : IResultsFilter
{
    public IAsyncEnumerable<SearchResult> Filter(IAsyncEnumerable<SearchResult> input) =>
        input.ThrowIfNull().Distinct(x => x.Url);
}