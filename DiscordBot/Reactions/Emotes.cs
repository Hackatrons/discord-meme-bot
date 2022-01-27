using Discord;

namespace DiscordBot.Reactions;

internal static class Emotes
{
    public static readonly IEmote Delete = Emoji.Parse("❌");
    public static readonly IEmote Repeat = Emoji.Parse("🔂");
    public static readonly IEmote ThumbsUp = Emoji.Parse("👍");
    public static readonly IEmote ThumbsDown = Emoji.Parse("👎");
}