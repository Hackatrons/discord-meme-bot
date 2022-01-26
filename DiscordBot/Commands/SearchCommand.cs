using Discord.Interactions;
using DiscordBot.Caching;
using DiscordBot.Filters;
using DiscordBot.Pushshift;
using JetBrains.Annotations;

namespace DiscordBot.Commands;

[UsedImplicitly]
// commands must be public classes for discord.net to use them
public class SearchCommand : BaseSearchCommand
{
    public SearchCommand(
        ResultsCache cache,
        AggregateFilter filter,
        IHttpClientFactory httpClientFactory) : base(cache, filter, httpClientFactory) { }

    [UsedImplicitly]
    [SlashCommand("search", "Search for anything (can include both sfw and nsfw results).")]
    public Task Execute(string query) => Search(query);

    protected override PushshiftQuery BuildBaseQuery(string query)
    {
        return new PushshiftQuery()
            .Search(query);
    }
}