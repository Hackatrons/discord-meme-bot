using Discord.Interactions;
using DiscordBot.Configuration;
using DiscordBot.Language;
using JetBrains.Annotations;

namespace DiscordBot.Commands;

/// <summary>
/// A slash command that provides a discord invite link for this bot.
/// </summary>
[UsedImplicitly]
public class InviteLinkCommand : InteractionModuleBase<SocketInteractionContext>
{
    readonly DiscordSettings _settings;

    public InviteLinkCommand(DiscordSettings settings) => _settings = settings.ThrowIfNull();

    [UsedImplicitly]
    [SlashCommand("invitelink", "Returns an invite link for this bot.")]
    public async Task Link() => await RespondAsync(_settings.InviteLink);
}