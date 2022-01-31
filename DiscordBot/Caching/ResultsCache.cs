using DiscordBot.Models;
using Microsoft.Extensions.Caching.Memory;

namespace DiscordBot.Caching;

/// <summary>
/// A cache for storing search result sets.
/// </summary>
public class ResultsCache
{
    // TODO: move to config
    static readonly TimeSpan CacheDuration = TimeSpan.FromHours(8);

    readonly MemoryCache _cache = new(new MemoryCacheOptions());

    /// <summary>
    /// Retrieves an existing or adds a new search results factory for the given command parameters.
    /// </summary>
    public IAsyncEnumerator<SearchResult> GetOrAdd(
        ulong channelId,
        string commandName,
        string arguments,
        Func<IAsyncEnumerator<SearchResult>> resultsFactory)
    {
        var key = CacheKey(channelId, commandName, arguments);

        return _cache.GetOrCreate(key, x =>
        {
            x.SetAbsoluteExpiration(CacheDuration);
            return resultsFactory();
        });
    }

    static string CacheKey(ulong channelId, string commandName, string arguments) => 
        $"{channelId}-{commandName}-{arguments}";
}