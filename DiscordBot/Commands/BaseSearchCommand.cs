using Discord.Interactions;
using DiscordBot.Language;
using DiscordBot.Messaging;
using DiscordBot.Queries;
using DiscordBot.Services;

namespace DiscordBot.Commands;

/// <summary>
/// Base class for search related commands.
/// Note: commands must be public classes for discord.net to use them
/// </summary>
public abstract class BaseSearchCommand : InteractionModuleBase<SocketInteractionContext>
{
    readonly EmoticonsHandler _emoticonsHandler;
    readonly RepeatCommandHandler _repeatCommandHandler;
    readonly BaseQueryHandler _queryHandler;

    protected BaseSearchCommand(
        BaseQueryHandler queryHandler,
        EmoticonsHandler emoticonsHandler,
        RepeatCommandHandler repeatCommandHandler)
    {
        _emoticonsHandler = emoticonsHandler;
        _queryHandler = queryHandler.ThrowIfNull();
        _repeatCommandHandler = repeatCommandHandler.ThrowIfNull();
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
            await FollowupAsync(finished ? BotMessage.NoMoreResultsMessage : BotMessage.NoResultsMessage);
            return;
        }

        var message = await FollowupAsync(result.Url);
        await _emoticonsHandler.AddResultReactions(message);
        await _repeatCommandHandler.Watch(message, _queryHandler, query);
    }
}