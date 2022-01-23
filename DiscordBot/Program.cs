using DiscordBot;

var startup = new Startup();
await startup.StartAsync();

Console.ReadLine();

await startup.StopAsync();
