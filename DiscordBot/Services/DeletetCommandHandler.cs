using Discord;
using Discord.WebSocket;
using DiscordBot.Language;
using DiscordBot.Reactions;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Services;

/// <summary>
/// Handler for delete commands.
/// </summary>
internal class DeleteCommandHandler : IInitialise
{
    readonly ILogger _logger;
    readonly DiscordSocketClient _client;

    public DeleteCommandHandler(
        DiscordSocketClient client,
        ILogger<RepeatCommandHandler> logger)
    {
        _client = client.ThrowIfNull();
        _logger = logger.ThrowIfNull();
    }

    public void Initialise()
    {
        _client.ReactionAdded += OnReactionAdded;
    }

    public void Dispose()
    {
        _client.ReactionAdded -= OnReactionAdded;
    }

    async Task OnReactionAdded(
        Cacheable<IUserMessage, ulong> cachedMessage,
        Cacheable<IMessageChannel, ulong> cachedChannel,
        SocketReaction reaction)
    {
        // ignore own reactions
        if (reaction.UserId == _client.CurrentUser.Id)
            return;

        if (!Emotes.Delete.Name.Equals(reaction.Emote.Name))
            return;

        var message = await cachedMessage.GetOrDownloadAsync();

        // if we are not the author of this message, then bail
        if (message.Author.Id != _client.CurrentUser.Id)
            return;

        _logger.LogInformation("Deleting message {id} at the request of {user}",
            cachedMessage.Id,
            reaction.User.IsSpecified ? reaction.User.Value : reaction.UserId);

        await message.DeleteAsync();
    }
}