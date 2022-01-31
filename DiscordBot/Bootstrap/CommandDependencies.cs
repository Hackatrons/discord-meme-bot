using DiscordBot.Caching;
using DiscordBot.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Bootstrap;

/// <summary>
/// Extension methods to configure discord command dependencies.
/// </summary>
internal static class CommandDependencies
{
    /// <summary>
    /// Adds discord commands and handlers to the container.
    /// </summary>
    public static IServiceCollection AddCommands(this IServiceCollection services) => services
        .AddSingleton<DiscordLogger>()
        .AddSingleton<CommandHandler>()
        .AddSingleton<RepeatCommandHandler>()
        .AddSingleton<DeleteCommandHandler>()
        .AddSingleton<ResultsCache>()
        .AddSingleton<RepeatCommandCache>()
        .AddSingleton(x => new IInitialise[]
        {
            x.GetRequiredService<DiscordLogger>(),
            x.GetRequiredService<CommandHandler>(),
            x.GetRequiredService<RepeatCommandHandler>(),
            x.GetRequiredService<DeleteCommandHandler>()
        });
}