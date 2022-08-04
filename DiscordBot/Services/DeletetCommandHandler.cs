using Discord;
using Discord.WebSocket;
using DiscordBot.Caching;
using DiscordBot.Language;
using DiscordBot.Reactions;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Services;

/// <summary>
/// Handler for delete commands.
/// </summary>
public class DeleteCommandHandler : IInitialise
{
    readonly ILogger _logger;
    readonly DiscordSocketClient _client;
    readonly ICache _cache;

    public DeleteCommandHandler(
        DiscordSocketClient client,
        ICache cache,
        ILogger<RepeatCommandHandler> logger)
    {
        _client = client.ThrowIfNull();
        _cache = cache.ThrowIfNull();
        _logger = logger.ThrowIfNull();
    }

    /// <summary>
    /// Registers a message to be watched for the delete command.
    /// </summary>
    public async Task Watch(IUserMessage message)
    {
        await _cache.Set(CacheKey(message.Id), true);
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

        // it's more likely that the channel has been cached than the message
        // so it's faster to try this first
        var isWatched = await _cache.GetAndPurge<bool?>(CacheKey(cachedMessage.Id));
        if (isWatched.GetValueOrDefault() && cachedChannel.HasValue)
        {
            var channel = await cachedChannel.GetOrDownloadAsync();
            await channel.DeleteMessageAsync(cachedMessage.Id);
        }
        // channel not in our cache so fallback to downloading the message
        else
        {
            var message = await cachedMessage.GetOrDownloadAsync();

            // small chance the message has already been deleted by someone else
            if (message == null)
                return;

            // if we are not the author of this message, then bail
            if (message.Author.Id != _client.CurrentUser.Id)
                return;

            await message.DeleteAsync();
        }

        _logger.LogInformation("Deleted message {id} at the request of {user}",
            cachedMessage.Id,
            reaction.User.IsSpecified ? reaction.User.Value : reaction.UserId);
    }

    static string CacheKey(ulong messageId) => $"CanDeleteMessage-{messageId}";
}