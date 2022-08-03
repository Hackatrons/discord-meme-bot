using Discord;
using DiscordBot.Reactions;
using DiscordBot.Threading;

namespace DiscordBot.Services;

/// <summary>
/// Handler for adding emoticons.
/// </summary>
public class EmoticonsHandler
{
    static readonly IEmote[] ResultEmotes =
    {
        Emotes.Delete,
        Emotes.Repeat
    };

    /// <summary>
    /// Adds the standard set of search result emoticons
    /// </summary>
    public Task AddResultReactions(IUserMessage message)
    {
        // adding reactions is very slow, so do this in a background task
        message.AddReactionsAsync(ResultEmotes).Forget();
        return Task.CompletedTask;
    }
}