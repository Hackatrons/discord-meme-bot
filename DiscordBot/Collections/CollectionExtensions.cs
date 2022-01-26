using DiscordBot.Numerics;

namespace DiscordBot.Collections;

public static class CollectionExtensions
{
    /// <summary>
    /// Randomly shuffles a list. Enumerates the entire async enumerable.
    /// </summary>
    public static async IAsyncEnumerable<T> Shuffle<T>(this IAsyncEnumerable<T> source)
    {
        // just a basic Fisher-Yates shuffle
        var list = await source.ToListAsync();
        var n = list.Count;
        while (n > 1)
        {
            n--;
            var k = ThreadSafeRandom.Random.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }

        foreach (var item in list)
            yield return item;
    }
}