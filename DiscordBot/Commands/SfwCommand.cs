using Discord.Interactions;
using DiscordBot.Queries;
using DiscordBot.Services;
using JetBrains.Annotations;

namespace DiscordBot.Commands;

/// <summary>
/// A search command for SFW content.
/// </summary>
[UsedImplicitly]
public class SfwCommand : BaseSearchCommand
{
    public SfwCommand(
        SfwQueryHandler queryHandler, 
        RepeatCommandHandler repeatCommandHandler,
        DeleteCommandHandler deleteCommandHandler)
        : base(queryHandler, repeatCommandHandler, deleteCommandHandler) { }

    [UsedImplicitly]
    [SlashCommand("sfw", "Search for only sfw results.")]
    public async Task Execute(string query) => await Search(query);
}