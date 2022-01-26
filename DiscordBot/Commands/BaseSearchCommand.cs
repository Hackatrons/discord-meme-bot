using Discord.Interactions;
using DiscordBot.Caching;
using DiscordBot.Filters;
using DiscordBot.Language;
using DiscordBot.Models;
using DiscordBot.Pushshift;
using DiscordBot.Pushshift.Models;

namespace DiscordBot.Commands;

// commands must be public classes for discord.net to use them
public abstract class BaseSearchCommand : InteractionModuleBase<SocketInteractionContext>
{
    const int SearchResultLimit = 200;

    readonly ResultsCache _cache;
    readonly IHttpClientFactory _httpClientFactory;
    readonly AggregateFilter _filter;
    readonly string _typeName;

    protected BaseSearchCommand(
        ResultsCache cache,
        AggregateFilter filter,
        IHttpClientFactory httpClientFactory)
    {
        _cache = cache.ThrowIfNull();
        _httpClientFactory = httpClientFactory.ThrowIfNull();
        _filter = filter.ThrowIfNull();
        _typeName = GetType().Name;
    }

    protected abstract PushshiftQuery BuildBaseQuery(string query);

    protected async Task Search(string query)
    {
        // we only get 3 seconds to respond before discord times out our request
        // but we can defer the response and have up to 15mins to provide a follow up response
        await DeferAsync();

        var psQuery = BuildBaseQuery(query);

        var results = _cache.GetOrAdd(
            Context.Channel.Id,
            _typeName,
            query,
            () => Search(psQuery));

        if (!await results.MoveNextAsync())
            await FollowupAsync("No more results");
        else
            await FollowupAsync(results.Current.Url);
    }

    async IAsyncEnumerator<SearchResult> Search(PushshiftQuery query)
    {
        var results = await PerformSearch(query);
        var filtered = _filter.Filter(results.ToAsyncEnumerable());

        await foreach (var item in filtered)
            yield return item;
    }

    async Task<IEnumerable<SearchResult>> PerformSearch(PushshiftQuery baseQuery)
    {
        using var httpClient = _httpClientFactory.CreateClient();
        // mix 2 search results in together:
        // 1. ordered by highest score
        // 2. ordered by most recent
        // I think this is a good mixture of results to include
        var mostRecentTask = baseQuery
            .Clone()
            .Limit(SearchResultLimit)
            .Sort(SortType.CreatedDate, SortDirection.Descending)
            .Fields<PushshiftResult>()
            .Execute(httpClient);

        var highestScoreTask = baseQuery
            .Clone()
            .Limit(SearchResultLimit)
            .Sort(SortType.Score, SortDirection.Descending)
            .Fields<PushshiftResult>()
            .Execute(httpClient);

        await Task.WhenAll(mostRecentTask, highestScoreTask);

        httpClient.Dispose();

        var mostRecent = await mostRecentTask;
        var highestScore = await highestScoreTask;
        var combined = mostRecent
            .UnionBy(highestScore, x => x.Url)
            .Select(SearchResult.FromPushshift);

        return combined;
    }
}