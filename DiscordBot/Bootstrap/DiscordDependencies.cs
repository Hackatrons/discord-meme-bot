using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Bootstrap;

/// <summary>
/// Extension methods to configure discord.net dependencies.
/// </summary>
internal static class DiscordDependencies
{
    /// <summary>
    /// Adds discord.net services to the container.
    /// </summary>
    public static IServiceCollection AddDiscord(this IServiceCollection services) => services
        .AddSingleton(new DiscordSocketConfig
        {
            LogLevel = LogSeverity.Verbose,
            GatewayIntents =
                GatewayIntents.Guilds |
                GatewayIntents.GuildMessages |
                GatewayIntents.GuildMessageReactions
        })
        .AddSingleton<DiscordSocketClient>()
        .AddSingleton(new InteractionServiceConfig
        {
            LogLevel = LogSeverity.Verbose
        })
        .AddSingleton<InteractionService>();
}