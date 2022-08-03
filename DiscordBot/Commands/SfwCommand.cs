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
    public SfwCommand(SfwQueryHandler queryHandler, EmoticonsHandler emoticonsHandler, RepeatCommandHandler repeatCommandHandler)
        : base(queryHandler, emoticonsHandler, repeatCommandHandler) { }

    [UsedImplicitly]
    [SlashCommand("sfw", "Search for only sfw results.")]
    public Task Execute(string query) => Search(query);
}