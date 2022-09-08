using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Caching;
using DiscordBot.Language;
using DiscordBot.Messaging;
using DiscordBot.Queries;

namespace DiscordBot.Commands;

/// <summary>
/// Base class for search related commands.
/// Note: commands must be public classes for discord.net to use them
/// </summary>
public abstract class BaseSearchCommand : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
{
    readonly QueryHandler _queryHandler;
    readonly ICache _cache;

    protected BaseSearchCommand(QueryHandler queryHandler, ICache cache)
    {
        _queryHandler = queryHandler.ThrowIfNull();
        _cache = cache.ThrowIfNull();
    }

    public async Task Search(string query)
    {
        // we only get 3 seconds to respond before discord times out our request
        // but we can defer the response and have up to 15mins to provide a follow up response
        await DeferAsync();

        // retrieve the next result (either from cache or executing the query)
        var (result, finished) = await _queryHandler.SearchNext(query, Context.Channel.Id);
        if (result == null)
        {
            await FollowupAsync(embed: finished ? BotMessage.NoMoreResults(query) : BotMessage.NoResults(query));
            return;
        }

        var message = await FollowupAsync(result.FinalUrl, components: BotMessage.ResultButtons);

        await RepeatCommand.Watch(message, _cache, _queryHandler, query);
    }
}