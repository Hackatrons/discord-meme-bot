using Discord;

namespace DiscordBot.Reactions;

/// <summary>
/// Emotes used by the bot.
/// </summary>
internal static class Emotes
{
    public static readonly IEmote Delete = Emoji.Parse("❌");
    public static readonly IEmote Repeat = Emoji.Parse("🔂");
}