using DiscordBot.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Bootstrap;

internal static class ConfigurationDependencies
{
    public static IServiceCollection AddSettings<T>(
        this IServiceCollection services,
        IConfiguration configuration)
        where T : class
    {
        services
            .AddOptions<T>()
            .Bind(configuration.GetRequiredSection(Config.SectionName<T>()))
            .ValidateDataAnnotations();

        return services;
    }
}