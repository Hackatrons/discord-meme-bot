using Discord.Interactions;
using DiscordBot.Caching;
using DiscordBot.Queries;
using JetBrains.Annotations;

namespace DiscordBot.Commands;

/// <summary>
/// A search command for general content (can be both sfw and nsfw).
/// </summary>
[UsedImplicitly]
public class SearchCommand : BaseSearchCommand
{
    public SearchCommand(
        SearchQueryHandler queryHandler,
        ICache cache)
        : base(queryHandler, cache) { }

    [UsedImplicitly]
    [SlashCommand("search", "Search for anything (can include both sfw and nsfw results).")]
    public async Task Execute(string query) => await Search(query);
}