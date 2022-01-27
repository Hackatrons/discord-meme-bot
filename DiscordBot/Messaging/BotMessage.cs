using Discord;
using DiscordBot.Configuration;

namespace DiscordBot.Messaging;

internal class BotMessage
{
    public static Embed Error(string title, string details)
        => new EmbedBuilder()
            .WithColor(BotColours.Error)
            .AddField(x =>
            {
                x.Name = title;
                x.Value = details;
            })
            .Build();

    public static Embed NotImplemented(string details)
        => new EmbedBuilder()
            .WithColor(BotColours.NotImplemented)
            .AddField(x =>
            {
                x.Name = "Not Implemented";
                x.Value = details;
            })
            .Build();
}