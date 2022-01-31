namespace DiscordBot.Configuration;

/// <summary>
/// Development/Testing configuration settings.
/// </summary>
public record TestingSettings
{
    /// <summary>
    /// The development/test server id.
    /// Required for publishing new slash commands within the 1 hour limitation.
    /// https://discordnet.dev/guides/int_framework/intro.html
    /// </summary>
    public ulong? TestServerId { get; init; }
}
