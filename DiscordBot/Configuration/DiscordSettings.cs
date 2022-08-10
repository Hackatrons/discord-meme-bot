using System.ComponentModel.DataAnnotations;

namespace DiscordBot.Configuration;

/// <summary>
/// Discord service configuration settings.
/// </summary>
public record DiscordSettings
{
    /// <summary>
    /// The bot token from https://discord.com/developers/applications/
    /// </summary>
    [Required]
    public string? Token { get; init; }

    /// <summary>
    /// The invitation link for a user to add this bot to a server.
    /// </summary>
    [Required]
    public string? InviteLink { get; init; }
}
