using Discord;
using DiscordBot.Configuration;

namespace DiscordBot.Messaging;

/// <summary>
/// Contains templated response messages.
/// </summary>
internal static class BotMessage
{
    /// <summary>
    /// Returns an error message embed from the specified error details.
    /// </summary>
    public static Embed Error(string title, string details)
        => new EmbedBuilder()
            .WithColor(BotColours.Error)
            .AddField(x =>
            {
                x.Name = title;
                x.Value = details;
            })
            .Build();

    /// <summary>
    /// Returns a non-implemented message embed from the specified message.
    /// </summary>
    public static Embed NotImplemented(string message)
        => new EmbedBuilder()
            .WithColor(BotColours.NotImplemented)
            .AddField(x =>
            {
                x.Name = "Not Implemented";
                x.Value = message;
            })
            .Build();
}