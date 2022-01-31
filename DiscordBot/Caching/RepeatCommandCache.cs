using System.Collections.Concurrent;

namespace DiscordBot.Caching;

/// <summary>
/// Stores references to a repeat command function based on message id.
/// </summary>
public class RepeatCommandCache
{
    readonly ConcurrentDictionary<ulong, Func<Task>> _actions = new();

    /// <summary>
    /// Adds a repeat command function for a specified message id.
    /// </summary>
    public void Add(ulong messageId, Func<Task> repeatCommand)
    {
        if (!_actions.TryAdd(messageId, repeatCommand))
            throw new InvalidOperationException($"Failed to add repeat command handler for message {messageId}");
    }

    /// <summary>
    /// Attempts to retrieve a repeat command function for a specified message id. 
    /// </summary>
    /// <returns>True if a repeat command was found, otherwise false.</returns>
    public bool TryGet(ulong messageId, out Func<Task>? repeatCommand)
        => _actions.TryGetValue(messageId, out repeatCommand);
}