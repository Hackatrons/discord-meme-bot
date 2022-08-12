using Discord.Interactions;
using DiscordBot.Queries;
using DiscordBot.Services;
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
        RepeatCommandHandler repeatCommandHandler,
        DeleteCommandHandler deleteCommandHandler)
        : base(queryHandler, repeatCommandHandler, deleteCommandHandler) { }

    [UsedImplicitly]
    [SlashCommand("search", "Search for anything (can include both sfw and nsfw results).")]
    public async Task Execute(string query) => await Search(query);
}