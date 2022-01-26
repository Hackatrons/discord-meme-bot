using Discord;
using Discord.WebSocket;
using DiscordBot.Caching;
using DiscordBot.Language;
using DiscordBot.Reactions;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Services;

internal class RepeatCommandHandler : IDisposable
{
    readonly ILogger _logger;
    readonly DiscordSocketClient _client;
    readonly RepeatCommandCache _repeatCommandHandler;

    public RepeatCommandHandler(
        DiscordSocketClient client,
        RepeatCommandCache repeatCommandHandler,
        ILogger<RepeatCommandHandler> logger)
    {
        _client = client.ThrowIfNull();
        _repeatCommandHandler = repeatCommandHandler.ThrowIfNull();
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

        if (!Emotes.Repeat.Name.Equals(reaction.Emote.Name))
            return;

        if (!_repeatCommandHandler.TryGet(cachedMessage.Id, out var repeatCommand))
            _logger.LogWarning("Missing repeat command handler for message {id}", cachedMessage.Id);

        await repeatCommand!();
    }
}