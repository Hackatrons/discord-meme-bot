using Discord;
using DiscordBot.Configuration;

namespace DiscordBot.Messaging;

/// <summary>
/// Contains templated response messages.
/// </summary>
internal static class BotMessage
{
    public const string DeleteButtonId = "delete";
    public const string RepeatButtonId = "repeat";

    /// <summary>
    /// The delete/repeat buttons to add with each result message.
    /// </summary>
    public static readonly MessageComponent ResultButtons = new ComponentBuilder()
        .WithButton(label: "Delete", customId: DeleteButtonId, style: ButtonStyle.Danger)
        .WithButton(label: "Repeat", customId: RepeatButtonId, style: ButtonStyle.Primary)
        .Build();

    /// <summary>
    /// Returns an error message embed from the specified error details.
    /// </summary>
    public static Embed Error(string title, string details)
        => new EmbedBuilder()
            .WithColor(BotColours.Error)
            .WithTitle(title)
            .WithDescription(details)
            .Build();

    /// <summary>
    /// Returns the no results warning message.
    /// </summary>
    public static Embed NoResults(string query)
        => new EmbedBuilder()
            .WithColor(BotColours.Warning)
            .WithTitle("Nothing found")
            .WithDescription($"No results found for query '{query}'.")
            .Build();

    /// <summary>
    /// Returns the no more results warning message.
    /// </summary>
    public static Embed NoMoreResults(string query)
        => new EmbedBuilder()
            .WithColor(BotColours.Warning)
            .WithTitle("None left")
            .WithDescription($"No more results left for query '{query}'.")
            .Build();
}