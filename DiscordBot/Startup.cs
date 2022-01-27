using DiscordBot.Bootstrap;
using DiscordBot.Configuration;
using DiscordBot.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

        FireAndForgetExtensions.Logger = _provider.GetRequiredService<ILogger<Task>>();
    }

    public Task StartAsync() => _provider
        .GetRequiredService<Bot>()
        .StartAsync();

    public Task StopAsync() => _provider
        .GetRequiredService<Bot>()
        .StopAsync();

    static IServiceProvider BuildServices(IServiceCollection services) => services
        .AddHttpClient()
        .AddDiscord()
        .AddCommands()
        .AddResultsFilters()
        .AddSingleton<Bot>()
        .BuildServiceProvider();

    static IServiceCollection BuildLogging(IConfiguration configuration, IServiceCollection services) => services
        .AddLogging(builder =>
        {
            var serilog = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            builder.AddSerilog(
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
            .AddSettings<DiscordSettings>(config)
            .AddSettings<TestingSettings>(config);

        return config;
    }
}