namespace DiscordBot.Numerics;

/// <summary>
/// Provides an instance of Random that is local to each thread, and is thus threadsafe
/// </summary>
public static class ThreadSafeRandom
{
    /// <summary>
    /// Thread-safe random instance.
    /// </summary>
    public static Random Random => ThreadRandom.Value ?? throw new InvalidOperationException();

    static readonly ThreadLocal<Random> ThreadRandom = new(() =>
        new Random(
            // provide some initial seed value
            // https://stackoverflow.com/questions/38507265/why-initialize-a-new-random-with-uncheckedenvironment-tickcount-31
            unchecked(Environment.TickCount * 31 + Environment.CurrentManagedThreadId)));
}