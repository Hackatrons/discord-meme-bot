using DiscordBot.Language;
using StackExchange.Redis;

namespace DiscordBot.Caching;

/// <summary>
/// Redis cache service.
/// </summary>
public class RedisCache : ICache
{
    readonly IDatabase _cache;

    public RedisCache(IDatabase cache)
    {
        _cache = cache.ThrowIfNull();
    }

    public async Task<string?> Get(string key) => await _cache.StringGetAsync(key);

    public async Task<string?> GetAndPurge( string key) => await _cache.StringGetDeleteAsync(key);

    public async Task Set(string key, string value, TimeSpan? expiry = null) => await _cache.StringSetAsync(key, value, expiry);
}