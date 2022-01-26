using System.Runtime.Loader;
using DiscordBot;

try
{
    var endSignal = new ManualResetEventSlim();
    AssemblyLoadContext.Default.Unloading += _ =>
    {
        endSignal.Set();
    };

    var startup = new Startup();
    await startup.StartAsync();

    Console.ReadLine();

    await startup.StopAsync();
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}
