using Discord.Interactions;
using DiscordBot.Language;
using DiscordBot.Messaging;
using DiscordBot.Queries;
using DiscordBot.Reactions;
using DiscordBot.Services;
using DiscordBot.Threading;

namespace DiscordBot.Commands;

/// <summary>
/// Base class for search related commands.
/// Note: commands must be public classes for discord.net to use them
/// </summary>
public abstract class BaseSearchCommand : InteractionModuleBase<SocketInteractionContext>
{
    readonly RepeatCommandHandler _repeatCommandHandler;
    readonly DeleteCommandHandler _deleteCommandHandler;
    readonly QueryHandler _queryHandler;

    protected BaseSearchCommand(
        QueryHandler queryHandler,
        RepeatCommandHandler repeatCommandHandler,
        DeleteCommandHandler deleteCommandHandler)
    {
        _queryHandler = queryHandler.ThrowIfNull();
        _deleteCommandHandler = deleteCommandHandler.ThrowIfNull();
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
            await FollowupAsync(embed: finished ? BotMessage.NoMoreResults(query) : BotMessage.NoResults(query));
            return;
        }

        var message = await FollowupAsync(result.FinalUrl);

        await Task.WhenAll(
            Emotes.AddResultReactions(message),
            _repeatCommandHandler.Watch(message, _queryHandler, query),
            _deleteCommandHandler.Watch(message));
    }
}