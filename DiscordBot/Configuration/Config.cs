namespace DiscordBot.Configuration;

/// <summary>
/// Extension methods for configuration classes.
/// </summary>
internal class Config
{
    /// <summary>
    /// Returns the configuration section name for a configuration class type.
    /// The convention is simply the type name without the word "Settings".
    /// e.g. "DiscordSettings" becomes "Discord", "BotSettings" becomes "Bot".
    /// </summary>
    public static string SectionName<T>() =>
        typeof(T)
            .Name
            .Replace("Settings", "");
}