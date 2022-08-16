using Discord;

namespace DiscordBot.Reactions;

/// <summary>
/// Emotes used by the bot.
/// </summary>
internal static class Emotes
{
    public static readonly IEmote Delete = Emoji.Parse("❌");
    public static readonly IEmote Repeat = Emoji.Parse("🔂");

    static readonly IEmote[] ResultEmotes =
    {
        Delete,
        Repeat
    };

    /// <summary>
    /// Adds the standard set of search result emoticons to the message.
    /// </summary>
    public static async Task AddResultReactions(IUserMessage message)
    {
        await message.AddReactionsAsync(ResultEmotes);
    }
}