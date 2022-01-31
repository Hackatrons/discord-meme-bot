using DiscordBot.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Bootstrap;

/// <summary>
/// Extension methods to configure result filters.
/// </summary>
internal static class ResultsFilterDependencies
{
    /// <summary>
    /// Adds result filters to the container.
    /// </summary>
    public static IServiceCollection AddResultFilters(this IServiceCollection services) => services
        .AddSingleton<DomainBlacklistFilter>()
        .AddSingleton<DuplicateFilter>()
        .AddSingleton<EmbeddableMediaFilter>()
        .AddSingleton<UrlCheckFilter>()
        .AddSingleton<AggregateFilter>()
        .AddSingleton<RandomiserFilter>();
}