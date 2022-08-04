namespace DiscordBot.Caching;

/// <summary>
/// Represents a basic cache to store and retrieve data.
/// </summary>
public interface ICache
{
    /// <summary>
    /// Retrieves an item from the cache, or null if it doesn't exist.
    /// </summary>
    Task<string?> Get(string key);

    /// <summary>
    /// Retrieves an item from the cache and purges it.
    /// </summary>
    Task<string?> GetAndPurge(string key);

    /// <summary>
    /// Sets an item in the cache.
    /// </summary>
    Task Set(string key, string value, TimeSpan? expiry = null);
}