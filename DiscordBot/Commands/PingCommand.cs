using Discord.Interactions;
using JetBrains.Annotations;

namespace DiscordBot.Commands;

[UsedImplicitly]
// we'll take care of command registration in the CommandHandler
// to avoid duplicate commands displaying in discord
[DontAutoRegister]
public class PingCommand : InteractionModuleBase<SocketInteractionContext>
{
    [UsedImplicitly]
    [SlashCommand("ping", "Responds with pong if the bot is alive.")]
    public async Task Ping()
    {
        await RespondAsync("Pong");
    }
}