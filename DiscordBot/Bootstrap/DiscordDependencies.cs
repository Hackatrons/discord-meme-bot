using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Bootstrap;

internal static class DiscordDependencies
{
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