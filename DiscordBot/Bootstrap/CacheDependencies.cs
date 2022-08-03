using DiscordBot.Caching;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace DiscordBot.Bootstrap;

/// <summary>
/// Extension methods to configure caching dependencies.
/// </summary>
internal static class CacheDependencies
{
    /// <summary>
    /// Adds caching services to the container.
    /// </summary>
    public static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("cache");
        var redis = ConnectionMultiplexer.Connect(connectionString);
        services.AddScoped(_ => redis.GetDatabase());

        return services
            .AddSingleton<ResultsCache>()
            .AddSingleton<ICache, RedisCache>();
    }
}