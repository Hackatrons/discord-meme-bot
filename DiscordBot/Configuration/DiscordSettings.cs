using System.ComponentModel.DataAnnotations;

namespace DiscordBot.Configuration;

internal class DiscordSettings
{
    [Required]
    public string? Token { get; set; }
}
