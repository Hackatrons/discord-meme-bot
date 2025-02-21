﻿using DiscordBot.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DiscordBot.Bootstrap;

/// <summary>
/// Extension methods to configure configuration dependencies.
/// </summary>
internal static class ConfigurationDependencies
{
    /// <summary>
    /// Adds an options class to the container and binds it to a configuration section based on the type name.
    /// e.g. "DiscordSettings" will bind to a configuration section called "Discord".
    /// </summary>
    public static IServiceCollection AddSettings<T>(
        this IServiceCollection services,
        IConfiguration configuration)
        where T : class
    {
        services
            .AddOptions<T>()
            .Bind(configuration.GetRequiredSection(Config.SectionName<T>()))
            .ValidateDataAnnotations();

        services.AddScoped(s => s.GetRequiredService<IOptions<T>>().Value);

        return services;
    }
}