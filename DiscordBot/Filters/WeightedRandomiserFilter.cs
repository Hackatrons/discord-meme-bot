using DiscordBot.Collections;
using DiscordBot.Models;
using DiscordBot.Ranking;

namespace DiscordBot.Filters
{
    public class WeightedRandomiserFilter : IResultsFilter
    {
        public IAsyncEnumerable<SearchResult> Filter(IAsyncEnumerable<SearchResult> input)
            => input.WeightedShuffle(x => x.Rank());
    }
}
