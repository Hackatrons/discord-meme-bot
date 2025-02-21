﻿using System.Runtime.InteropServices;
using System.Runtime.Loader;
using DiscordBot;

try
{
    Console.WriteLine("Starting...");

    var runToken = new CancellationTokenSource();

    // kill signal listeners
    AssemblyLoadContext.Default.Unloading += _ => runToken.Cancel();
    AppDomain.CurrentDomain.ProcessExit += (_, _) => runToken.Cancel();
    Console.CancelKeyPress += (_, _) => runToken.Cancel();
    _ = PosixSignalRegistration.Create(PosixSignal.SIGTERM, _ => runToken.Cancel());

    var startup = new Startup();

    // start the app
    await startup.StartAsync();

    try
    {
        // wait for a kill signal
        await Task.Delay(Timeout.Infinite, runToken.Token);
    }
    catch (TaskCanceledException) { }

    Console.WriteLine("Stopping...");

    // stop the app
    await startup.StopAsync();

    Environment.Exit(0);
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}