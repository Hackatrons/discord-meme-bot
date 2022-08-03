using DiscordBot.Language;

namespace DiscordBot.Caching;

public class RepeatCommandData
{
    public string Type { get; init; }
    public string Query { get; init; }

    public RepeatCommandData(string query, string type)
    {
        Query = query.ThrowIfNullOrWhitespace();
        Type = type.ThrowIfNullOrWhitespace();
    }
}