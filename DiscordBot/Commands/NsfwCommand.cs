using Discord.Interactions;
using DiscordBot.Queries;
using DiscordBot.Services;
using JetBrains.Annotations;

namespace DiscordBot.Commands;

/// <summary>
/// A search command for NSFW content.
/// </summary>
[UsedImplicitly]
public class NsfwCommand : BaseSearchCommand
{
    public NsfwCommand(
        NsfwQueryHandler queryHandler,  
        RepeatCommandHandler repeatCommandHandler,
        DeleteCommandHandler deleteCommandHandler)
        : base(queryHandler, repeatCommandHandler, deleteCommandHandler) { }

    [UsedImplicitly]
    [SlashCommand("nsfw", "Search for only nsfw results.")]
    public async Task Execute(string query) => await Search(query);
}