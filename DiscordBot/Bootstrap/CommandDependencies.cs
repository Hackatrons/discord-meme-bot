using DiscordBot.Commands;
using DiscordBot.Filters;
using DiscordBot.Logging;
using DiscordBot.Queries;
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
        .AddSingleton<RepeatCommand>()
        .AddSingleton<DeleteCommand>()
        .AddSingleton(x => new IInitialise[]
        {
            x.GetRequiredService<DiscordLogger>(),
            x.GetRequiredService<CommandHandler>()
        })
        .AddSingleton<QueryMultiplexer>()
        .AddSingleton<ResultProber>()
        .AddSingleton<NsfwQueryHandler>()
        .AddSingleton<SfwQueryHandler>()
        .AddSingleton<SearchQueryHandler>();
}