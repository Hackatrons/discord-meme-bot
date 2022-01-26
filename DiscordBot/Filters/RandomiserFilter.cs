using DiscordBot.Collections;
using DiscordBot.Models;

namespace DiscordBot.Filters
{
    public class RandomiserFilter : IResultsFilter
    {
        public IAsyncEnumerable<SearchResult> Filter(IAsyncEnumerable<SearchResult> input)
            => input.Shuffle();
    }
}
