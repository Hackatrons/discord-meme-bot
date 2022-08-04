using DiscordBot.Configuration;
using DiscordBot.Language;
using DiscordBot.Models;

namespace DiscordBot.Caching;

/// <summary>
/// A cache for storing search result sets.
/// </summary>
public class ResultsCache
{
    readonly CacheSettings _settings;
    readonly ICache _cache;

    public ResultsCache(ICache cache, CacheSettings settings)
    {
        _cache = cache.ThrowIfNull();
        _settings = settings.ThrowIfNull();
    }

    /// <summary>
    /// Retrieves the existing search results for the the given command parameters, or null if there is nothing cached.
    /// </summary>
    public async Task<IReadOnlyCollection<SearchResult>?> Get(
        ulong channelId,
        string commandName,
        string arguments)
    {
        var key = CacheKey(channelId, commandName, arguments);

        return await _cache.Get<SearchResult[]>(key);
    }

    /// <summary>
    /// Stores the search results for the the given command parameters.
    /// </summary>
    public async Task Set(
        ulong channelId,
        string commandName,
        string arguments,
        IEnumerable<SearchResult> results)
    {
        var key = CacheKey(channelId, commandName, arguments);
        await _cache.Set(key, results, _settings.Duration);
    }

    static string CacheKey(ulong channelId, string commandName, string arguments) =>
        $"{channelId}-{commandName}-{arguments}";
}