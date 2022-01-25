using System.ComponentModel.DataAnnotations;

namespace DiscordBot.Configuration;

public record DiscordSettings
{
    [Required]
    public string? Token { get; init; }
    [Required]
    public string? InviteLink { get; init; }
}
