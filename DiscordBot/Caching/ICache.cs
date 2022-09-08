using System.Text.Json;

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
    /// Retrieves a json item from the cache and deserialises it.
    /// </summary>
    async Task<T?> Get<T>(string key)
    {
        var json = await Get(key);
        return json == null ? default : JsonSerializer.Deserialize<T>(json);
    }

    /// <summary>
    /// Sets an item in the cache.
    /// </summary>
    Task Set(string key, string value, TimeSpan? expiry = null);

    /// <summary>
    /// Serialises an item as json and then stores it in the cache.
    /// </summary>
    async Task Set<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value);
        await Set(key, json, expiry);
    }
}