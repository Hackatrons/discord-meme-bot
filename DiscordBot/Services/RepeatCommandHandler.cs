using Discord;
using Discord.WebSocket;
using DiscordBot.Caching;
using DiscordBot.Language;
using DiscordBot.Messaging;
using DiscordBot.Queries;
using DiscordBot.Reactions;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Services;

/// <summary>
/// Handler for repeating commands.
/// </summary>
public class RepeatCommandHandler : IInitialise
{
    readonly ILogger _logger;
    readonly IServiceProvider _serviceProvider;
    readonly ICache _cache;
    readonly DiscordSocketClient _client;
    readonly EmoticonsHandler _emoticonsHandler;
    readonly DeleteCommandHandler _deleteCommandHandler;

    public RepeatCommandHandler(
        DiscordSocketClient client,
        ILogger<RepeatCommandHandler> logger,
        IServiceProvider serviceProvider,
        ICache cache,
        EmoticonsHandler emoticonsHandler,
        DeleteCommandHandler deleteCommandHandler)
    {
        _cache = cache.ThrowIfNull();
        _serviceProvider = serviceProvider.ThrowIfNull();
        _client = client.ThrowIfNull();
        _logger = logger.ThrowIfNull();
        _emoticonsHandler = emoticonsHandler.ThrowIfNull();
        _deleteCommandHandler = deleteCommandHandler.ThrowIfNull();
    }

    /// <summary>
    /// Registers a message to be watched for the repeat command.
    /// </summary>
    public async Task Watch(IUserMessage message, BaseQueryHandler queryHandler, string query)
    {
        var repeatData = new RepeatCommandData(query, queryHandler.GetType().FullName!);

        await _cache.Set(message.Id.ToString(), JsonSerializer.Serialize(repeatData));
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

        var repeatDataString = await _cache.Get(cachedMessage.Id.ToString());
        if (string.IsNullOrEmpty(repeatDataString))
        {
            var message = await cachedMessage.GetOrDownloadAsync();

            // if it's not our message, ignore
            if (message.Author.Id != _client.CurrentUser.Id)
                return;

            _logger.LogWarning("Missing repeat command handler for message {id}", cachedMessage.Id);

            await message.ReplyAsync(embed: BotMessage.NotImplemented("Sorry, unable to repeat command as I've lost the original query context."));
            return;
        }

        var repeatData = JsonSerializer.Deserialize<RepeatCommandData>(repeatDataString);
        if (repeatData == null) return;

        var type = Type.GetType(repeatData.Type);
        if (type == null)
        {
            _logger.LogError("Unable to find query handler type '{type}'", repeatData.Type);
            return;
        }

        if (!type.IsAssignableTo(typeof(BaseQueryHandler)))
        {
            _logger.LogError("Type '{type}' is not a query handler type.", repeatData.Type);
            return;
        }

        var channel = await cachedChannel.GetOrDownloadAsync();
        var queryHandler = (BaseQueryHandler)_serviceProvider.GetRequiredService(type);

        await Repeat(repeatData.Query, queryHandler, channel);
    }

    async Task Repeat(string query, BaseQueryHandler handler, IMessageChannel channel)
    {
        // if this is a repeat command, then we'll get a cache hit
        // so our response time should be within 3 seconds
        using var state = channel.EnterTypingState();

        // retrieve the next result (either from cache or executing the query)
        var (result, finished) = await handler.SearchNext(query, channel.Id);
        if (result == null)
        {
            await channel.SendMessageAsync(finished ? BotMessage.NoMoreResultsMessage : BotMessage.NoResultsMessage);
            return;
        }

        var message = await channel.SendMessageAsync(result.Url);

        await Task.WhenAll(
            _emoticonsHandler.AddResultReactions(message),
            Watch(message, handler, query),
            _deleteCommandHandler.Watch(message));
    }
}