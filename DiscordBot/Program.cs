using System.Runtime.Loader;
using DiscordBot;

try
{
    var runToken = new CancellationTokenSource();

    // kill signal listeners
    AssemblyLoadContext.Default.Unloading += _ => runToken.Cancel();
    AppDomain.CurrentDomain.ProcessExit += (_, _) => runToken.Cancel();
    Console.CancelKeyPress += (_, _) => runToken.Cancel();

    var startup = new Startup();

    // start the app
    await startup.StartAsync();

    try
    {
        // wait for a kill signal
        await Task.Delay(Timeout.Infinite, runToken.Token);
    }
    catch (TaskCanceledException) { }

    // stop the app
    await startup.StopAsync();

    Environment.Exit(0);
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}
