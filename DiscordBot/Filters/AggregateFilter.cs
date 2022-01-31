using DiscordBot.Language;
using DiscordBot.Models;

namespace DiscordBot.Filters;

/// <summary>
/// Aggregates all search result filters one.
/// </summary>
public class AggregateFilter : IResultFilter
{
    readonly IResultFilter[] _filters;

    public AggregateFilter(
        DomainBlacklistFilter domainBlacklistFilter,
        DuplicateFilter duplicateFilter,
        EmbeddableMediaFilter embeddableMediaFilter,
        UrlCheckFilter urlCheckFilter,
        RandomiserFilter randomiserFilter)
    {
        _filters = new IResultFilter[]
        {
            // note that the order here matters
            // where we want to the filters which have the least amount of work to do run first
            // and filters that are more expensive at the tail end of the pipeline
            domainBlacklistFilter.ThrowIfNull(),
            duplicateFilter.ThrowIfNull(),
            embeddableMediaFilter.ThrowIfNull(),
            randomiserFilter.ThrowIfNull(),
            urlCheckFilter.ThrowIfNull(),
            // the URL can change after the check filter if a redirect response is returned
            // so slap in another duplicate check
            duplicateFilter.ThrowIfNull()
        };
    }

    public async IAsyncEnumerable<SearchResult> Filter(IAsyncEnumerable<SearchResult> input)
    {
        var filtered = _filters.Aggregate(
            input.ThrowIfNull(), 
            (current, filter) => filter.Filter(current));

        await foreach (var result in filtered)
            yield return result;
    }
}