using Discord.Interactions;
using DiscordBot.Caching;
using DiscordBot.Filters;
using DiscordBot.Pushshift;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Commands;

/// <summary>
/// A search command for general content (can be both sfw and nsfw).
/// </summary>
[UsedImplicitly]
public class SearchCommand : BaseSearchCommand
{
    public SearchCommand(
        ResultsCache cache,
        AggregateFilter filter,
        IHttpClientFactory httpClientFactory,
        RepeatCommandCache repeatCommandHandler,
        ILogger<SearchCommand> logger) : base(cache, filter, httpClientFactory, repeatCommandHandler, logger) { }

    [UsedImplicitly]
    [SlashCommand("search", "Search for anything (can include both sfw and nsfw results).")]
    public Task Execute(string query) => 
        ExecuteInternal(query);

    protected override PushshiftQuery BuildBaseQuery(string query) =>
        new PushshiftQuery()
            .Search(query);
}