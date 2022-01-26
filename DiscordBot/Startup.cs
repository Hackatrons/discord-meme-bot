using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Caching;
using DiscordBot.Commands;
using DiscordBot.Configuration;
using DiscordBot.Filters;
using DiscordBot.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

    public Task StartAsync() =>
        _provider
            .GetRequiredService<Bot>()
            .StartAsync();

    public Task StopAsync() =>
        _provider
            .GetRequiredService<Bot>()
            .StopAsync();

    static IServiceProvider BuildServices(IServiceCollection services) => services
        .AddSingleton(new DiscordSocketConfig
        {
            LogLevel = LogSeverity.Verbose,
            GatewayIntents =
                GatewayIntents.Guilds |
                GatewayIntents.GuildMessages
        })
        .AddSingleton<DiscordSocketClient>()
        .AddSingleton(new InteractionServiceConfig
        {
            LogLevel = LogSeverity.Verbose
        })
        .AddSingleton<InteractionService>()
        .AddSingleton<DiscordLogger>()
        .AddSingleton<CommandHandler>()
        .AddSingleton<ResultsCache>()
        .AddSingleton<Bot>()
        .AddResultsFilters()
        .AddHttpClient()
        .BuildServiceProvider();

    static IServiceCollection BuildLogging(IConfiguration configuration, IServiceCollection services) => services
        .AddLogging(loggingBuilder =>
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
            .AddUserSecrets<Startup>(true, true)
            .AddEnvironmentVariables()
            .Build();

        services
            .AddOptions<DiscordSettings>()
            .Bind(config.GetRequiredSection(Config.SectionName<DiscordSettings>()))
            .ValidateDataAnnotations();

        services
            .AddOptions<TestingSettings>()
            .Bind(config.GetRequiredSection(Config.SectionName<TestingSettings>()))
            .ValidateDataAnnotations();

        return config;
    }
}