using Discord;
using Discord.WebSocket;
using DiscordBot.Caching;
using DiscordBot.Language;
using DiscordBot.Messaging;
using DiscordBot.Queries;
using DiscordBot.Reactions;
using DiscordBot.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
    readonly DeleteCommandHandler _deleteCommandHandler;

    public RepeatCommandHandler(
        DiscordSocketClient client,
        ILogger<RepeatCommandHandler> logger,
        IServiceProvider serviceProvider,
        ICache cache,
        DeleteCommandHandler deleteCommandHandler)
    {
        _cache = cache.ThrowIfNull();
        _serviceProvider = serviceProvider.ThrowIfNull();
        _client = client.ThrowIfNull();
        _logger = logger.ThrowIfNull();
        _deleteCommandHandler = deleteCommandHandler.ThrowIfNull();
    }

    /// <summary>
    /// Registers a message to be watched for the repeat command.
    /// </summary>
    public async Task Watch(IUserMessage message, QueryHandler queryHandler, string query)
    {
        var repeatData = new RepeatCommandData(queryHandler.GetType().FullName!, query);

        await _cache.Set(message.Id.ToString(), repeatData);
    }

    public void Initialise()
    {
        _client.ReactionAdded += OnReactionAdded;
    }

    public void Dispose()
    {
        _client.ReactionAdded -= OnReactionAdded;
    }

    Task OnReactionAdded(
        Cacheable<IUserMessage, ulong> cachedMessage,
        Cacheable<IMessageChannel, ulong> cachedChannel,
        SocketReaction reaction)
    {
        // ignore own reactions
        if (reaction.UserId == _client.CurrentUser.Id)
            return Task.CompletedTask;

        if (!Emotes.Repeat.Name.Equals(reaction.Emote.Name))
            return Task.CompletedTask;

        // run in a background thread to avoid blocking the discord.net gateway task
        Task.Run(async () =>
        {
            var repeatData = await _cache.Get<RepeatCommandData>(cachedMessage.Id.ToString());

            if (repeatData == null)
            {
                var message = await cachedMessage.GetOrDownloadAsync();

                // if it's not our message, ignore
                if (message == null || message.Author.Id != _client.CurrentUser.Id)
                    return;

                _logger.LogWarning("Missing repeat command handler for message {id}", cachedMessage.Id);

                await message.ReplyAsync(embed: BotMessage.Error("Cannot repeat", "Sorry, unable to repeat command as I've lost the original query context."));
                return;
            }

            var type = Type.GetType(repeatData.Type);
            if (type == null)
            {
                _logger.LogError("Unable to find query handler type '{type}'", repeatData.Type);
                return;
            }

            if (!type.IsAssignableTo(typeof(QueryHandler)))
            {
                _logger.LogError("Type '{type}' is not a query handler type.", repeatData.Type);
                return;
            }

            var channel = await cachedChannel.GetOrDownloadAsync();
            var queryHandler = (QueryHandler)_serviceProvider.GetRequiredService(type);

            await Repeat(repeatData.Query, queryHandler, channel);
        }).Forget();

        return Task.CompletedTask;
    }

    async Task Repeat(string query, QueryHandler handler, IMessageChannel channel)
    {
        using var state = channel.EnterTypingState();

        // retrieve the next result (either from cache or executing the query)
        var (result, finished) = await handler.SearchNext(query, channel.Id);
        if (result == null)
        {
            await channel.SendMessageAsync(embed: finished ? BotMessage.NoMoreResults(query) : BotMessage.NoResults(query));
            return;
        }

        var message = await channel.SendMessageAsync(result.FinalUrl);

        await Task.WhenAll(
            Emotes.AddResultReactions(message),
            Watch(message, handler, query),
            _deleteCommandHandler.Watch(message));
    }
}