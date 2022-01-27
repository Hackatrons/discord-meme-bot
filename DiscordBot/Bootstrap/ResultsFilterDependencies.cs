using DiscordBot.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Bootstrap;

internal static class ResultsFilterDependencies
{
    public static IServiceCollection AddResultsFilters(this IServiceCollection services) => services
        .AddSingleton<DomainBlacklistFilter>()
        .AddSingleton<DuplicateFilter>()
        .AddSingleton<EmbeddableMediaFilter>()
        .AddSingleton<UrlCheckFilter>()
        .AddSingleton<AggregateFilter>()
        .AddSingleton<RandomiserFilter>();
}