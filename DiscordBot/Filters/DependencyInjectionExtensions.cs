using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Filters;

internal static class DependencyInjectionExtensions
{
    public static IServiceCollection AddResultsFilters(this IServiceCollection services) => services
        .AddSingleton<DomainBlacklistFilter>()
        .AddSingleton<DuplicateFilter>()
        .AddSingleton<EmbeddableMediaFilter>()
        .AddSingleton<UrlCheckFilter>()
        .AddSingleton<AggregateFilter>();
}