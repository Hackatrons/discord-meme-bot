namespace DiscordBot.Configuration;

internal class Config
{
    public static string SectionName<T>() =>
        typeof(T).Name
            .Replace("Settings", "");
}