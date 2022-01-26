using DiscordBot;

try
{
    var startup = new Startup();
    await startup.StartAsync();

    Console.ReadLine();

    await startup.StopAsync();
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}
