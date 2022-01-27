using Microsoft.Extensions.Logging;

namespace DiscordBot.Threading;

internal static class FireAndForgetExtensions
{
    public static ILogger? Logger = null;

    public static void Forget(this Task task)
    {
        _ = FireAndForget(task);
    }

    static async Task FireAndForget(Task task)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch(Exception ex)
        {
            // no one is listening to this task, so just log and ignore
            Logger?.LogError("Exception occurred in a fire and forget task: {error}", ex);
        }
    }
}