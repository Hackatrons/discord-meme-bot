using DiscordBot.Numerics;

namespace DiscordBot.Collections;

/// <summary>
/// Extension methods for collections.
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Randomly shuffles a list.
    /// Note: Enumerates the entire async enumerable.
    /// </summary>
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        // just a basic Fisher-Yates shuffle
        var list = source.ToList();
        for (var i = list.Count - 1; i > 0; i--)
        {
            var k = ThreadSafeRandom.Random.Next(i + 1);
            (list[k], list[i]) = (list[i], list[k]);
        }

        return list;
    }
}