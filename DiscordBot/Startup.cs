using Discord;
using Discord.WebSocket;
using DiscordBot.Configuration;
using DiscordBot.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;

namespace DiscordBot;

internal class Startup
{
    readonly IServiceProvider _provider;

    public Startup()
    {
        var services = new ServiceCollection();

        var config = BuildConfig(services);
        _ = BuildLogging(config, services);
        _provider = BuildServices(services);
    }

    public async Task StartAsync()
    {
        _ = _provider.GetRequiredService<DiscordSocketClientLogger>();

        var client = _provider.GetRequiredService<DiscordSocketClient>();
        var config = _provider.GetRequiredService<IOptions<DiscordSettings>>();

        await client.LoginAsync(TokenType.Bot, config.Value.Token);
        await client.StartAsync();
    }

    public async Task StopAsync()
    {
        var client = _provider.GetRequiredService<DiscordSocketClient>();

        await client.LogoutAsync();
        await client.StopAsync();
        await client.DisposeAsync();
    }

    static IServiceProvider BuildServices(IServiceCollection services) =>
        services
            .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose
            }))
            .AddSingleton<DiscordSocketClientLogger>()
            .BuildServiceProvider();

    static IServiceCollection BuildLogging(IConfiguration configuration, IServiceCollection services) =>
        services.AddLogging(loggingBuilder =>
        {
            var serilog = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            loggingBuilder.AddSerilog(
                serilog,
                true);
        });

    static IConfigurationRoot BuildConfig(IServiceCollection services)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", false, true)
            .AddUserSecrets<Startup>()
            .Build();

        services
            .AddOptions<DiscordSettings>()
            .Bind(config.GetRequiredSection(Config.SectionName<DiscordSettings>()))
            .ValidateDataAnnotations();

        return config;
    }
}