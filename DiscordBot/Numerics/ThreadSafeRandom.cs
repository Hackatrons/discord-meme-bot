namespace DiscordBot.Numerics;

/// <summary>
/// Provides an instance of Random that is local to each thread, and is thus threadsafe
/// </summary>
public static class ThreadSafeRandom
{
    public static Random Random => ThreadRandom.Value ?? throw new InvalidOperationException();

    static readonly ThreadLocal<Random> ThreadRandom = new(() =>
        new Random(unchecked(Environment.TickCount * 31 + Environment.CurrentManagedThreadId)));
}