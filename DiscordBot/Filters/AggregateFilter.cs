using DiscordBot.Language;
using DiscordBot.Models;

namespace DiscordBot.Filters;

public class AggregateFilter : IResultsFilter
{
    readonly DomainBlacklistFilter _domainBlacklistFilter;
    readonly DuplicateFilter _duplicateFilter;
    readonly EmbeddableMediaFilter _embeddableMediaFilter;
    readonly UrlCheckFilter _urlExistsFilter;

    public AggregateFilter(
        DomainBlacklistFilter domainBlacklistFilter,
        DuplicateFilter duplicateFilter,
        EmbeddableMediaFilter embeddableMediaFilter,
        UrlCheckFilter urlExistsFilter)
    {
        _domainBlacklistFilter = domainBlacklistFilter.ThrowIfNull();
        _duplicateFilter = duplicateFilter.ThrowIfNull();
        _embeddableMediaFilter = embeddableMediaFilter.ThrowIfNull();
        _urlExistsFilter = urlExistsFilter.ThrowIfNull();
    }

    public async IAsyncEnumerable<SearchResult> Filter(IAsyncEnumerable<SearchResult> input)
    {
        var filters = new IResultsFilter[]
        {
            // note that the order here matters
            // where we want to the filters which have the least amount of work to do run first
            // and filters that are more expensive at the tail end of the pipeline
            _domainBlacklistFilter,
            _duplicateFilter,
            _embeddableMediaFilter,
            _urlExistsFilter
        };

        var filtered = filters.Aggregate(input.ThrowIfNull(), (current, filter) => filter.Filter(current));

        await foreach (var result in filtered)
            yield return result;
    }
}