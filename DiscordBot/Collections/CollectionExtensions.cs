using DiscordBot.Language;
using DiscordBot.Numerics;

namespace DiscordBot.Collections;

public static class CollectionExtensions
{
    /// <summary>
    /// Performs a weighted shuffle on a list so that the results follow a probability distribution based on the key selector
    /// </summary>
    public static IAsyncEnumerable<T> WeightedShuffle<T>(this IAsyncEnumerable<T> list, Func<T, double> weightSelector)
    {
        weightSelector.ThrowIfNull(nameof(weightSelector));

        // using algorithm A from http://utopia.duth.gr/~pefraimi/research/data/2007EncOfAlg.pdf
        return list
            .ThrowIfNull()
            .OrderByDescending(x => Math.Pow(ThreadSafeRandom.Random.NextDouble(), 1.0 / weightSelector(x)));
    }

    /// <summary>
    /// Randomly shuffles a list.
    /// </summary>
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        // just a basic Fisher-Yates shuffle
        var list = new List<T>(source);
        var n = list.Count;
        while (n > 1)
        {
            n--;
            var k = ThreadSafeRandom.Random.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }

        return list;
    }
}