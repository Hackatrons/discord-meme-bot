using Discord.Interactions;
using JetBrains.Annotations;

namespace DiscordBot.Commands;

[UsedImplicitly]
public class PingCommand : InteractionModuleBase<SocketInteractionContext>
{
    [UsedImplicitly]
    [SlashCommand("ping", "Responds with pong if the bot is alive.")]
    public async Task Ping()
    {
        await RespondAsync("Pong");
    }
}