using System.Collections.Concurrent;

namespace DiscordBot.Caching;

public class RepeatCommandCache
{
    readonly ConcurrentDictionary<ulong, Func<Task>> _actions = new();

    public void Add(ulong messageId, Func<Task> repeatCommand)
    {
        if (!_actions.TryAdd(messageId, repeatCommand))
            throw new InvalidOperationException($"Failed to add repeat command handler for message {messageId}");
    }

    public bool TryGet(ulong messageId, out Func<Task>? repeatCommand)
        => _actions.TryGetValue(messageId, out repeatCommand);
}