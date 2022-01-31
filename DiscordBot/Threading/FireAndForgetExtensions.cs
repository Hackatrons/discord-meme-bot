using Microsoft.Extensions.Logging;

namespace DiscordBot.Threading;

/// <summary>
/// Extension methods for background tasks.
/// </summary>
internal static class FireAndForgetExtensions
{
    public static ILogger? Logger = null;

    /// <summary>
    /// Safely runs the task in the background without awaiting the result.
    /// </summary>
    public static void Forget(this Task task) =>
        _ = FireAndForget(task);

    static async Task FireAndForget(Task task)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // no one is listening to this task, so just log and ignore
            Logger?.LogError("Exception occurred in a fire and forget task: {error}", ex);
        }
    }
}