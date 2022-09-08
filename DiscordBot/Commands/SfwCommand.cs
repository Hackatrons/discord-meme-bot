using Discord.Interactions;
using DiscordBot.Caching;
using DiscordBot.Queries;
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
        ICache cache)
        : base(queryHandler, cache) { }

    [UsedImplicitly]
    [SlashCommand("sfw", "Search for only sfw results.")]
    public async Task Execute(string query) => await Search(query);
}