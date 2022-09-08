using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Caching;
using DiscordBot.Language;
using DiscordBot.Messaging;
using DiscordBot.Queries;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Commands;

/// <summary>
/// Handler for repeating commands.
/// </summary>
public class RepeatCommand : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    readonly IServiceProvider _serviceProvider;
    readonly ICache _cache;
    readonly ILogger _logger;

    public RepeatCommand(
        ILogger<RepeatCommand> logger,
        IServiceProvider serviceProvider,
        ICache cache)
    {
        _cache = cache.ThrowIfNull();
        _serviceProvider = serviceProvider.ThrowIfNull();
        _logger = logger.ThrowIfNull();
    }

    /// <summary>
    /// Registers a message to be watched for the repeat command.
    /// </summary>
    public static async Task Watch(IUserMessage message, ICache cache, QueryHandler queryHandler, string query)
    {
        var repeatData = new RepeatCommandData(queryHandler.GetType().FullName!, query);

        await cache.Set(message.Id.ToString(), repeatData);
    }

    [UsedImplicitly]
    [ComponentInteraction(BotMessage.RepeatButtonId)]
    public async Task Execute()
    {
        // acknowledge the interaction
        await Context.Interaction.DeferAsync();

        var repeatData = await _cache.Get<RepeatCommandData>(Context.Interaction.Message.Id.ToString());
        if (repeatData == null)
        {
            _logger.LogWarning("Missing repeat command handler for message '{id}'", Context.Interaction.Message.Id);

            await Context.Channel.SendMessageAsync(embed: BotMessage.Error("Cannot repeat", "Sorry, unable to repeat command as I've lost the original query context."));
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

        var queryHandler = (QueryHandler)_serviceProvider.GetRequiredService(type);

        await Repeat(repeatData.Query, queryHandler, Context.Channel);
    }

    async Task Repeat(string query, QueryHandler handler, IMessageChannel channel)
    {
        // retrieve the next result (either from cache or executing the query)
        var (result, finished) = await handler.SearchNext(query, channel.Id);
        if (result == null)
        {
            await channel.SendMessageAsync(embed: finished ? BotMessage.NoMoreResults(query) : BotMessage.NoResults(query));
            return;
        }

        var message = await channel.SendMessageAsync(result.FinalUrl, components: BotMessage.ResultButtons);

        await Watch(message, _cache, handler, query);
    }
}