using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Language;
using DiscordBot.Messaging;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Commands;

/// <summary>
/// Handler for delete commands.
/// </summary>
public class DeleteCommand : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    readonly ILogger _logger;

    public DeleteCommand(ILogger<RepeatCommand> logger)
    {
        _logger = logger.ThrowIfNull();
    }

    [UsedImplicitly]
    [ComponentInteraction(BotMessage.DeleteButtonId)]
    public async Task Execute()
    {
        // we still need to acknolwedge the interaction even though we're going to delete the message
        await Context.Interaction.DeferAsync();

        await Context.Interaction.DeleteOriginalResponseAsync();

        _logger.LogInformation("Deleted message '{id}' at the request of '{user}'", Context.Interaction.Message.CleanContent, Context.User.Username);
    }
}