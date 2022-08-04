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
        EmoticonsHandler emoticonsHandler, 
        RepeatCommandHandler repeatCommandHandler,
        DeleteCommandHandler deleteCommandHandler)
        : base(queryHandler, emoticonsHandler, repeatCommandHandler, deleteCommandHandler) { }

    [UsedImplicitly]
    [SlashCommand("search", "Search for anything (can include both sfw and nsfw results).")]
    public Task Execute(string query) => Search(query);
}