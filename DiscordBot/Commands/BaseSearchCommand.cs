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
    readonly HttpClient _httpClient;
    readonly AggregateFilter _filter;
    readonly string _typeName;

    protected BaseSearchCommand(
        ResultsCache cache,
        AggregateFilter filter,
        HttpClient client)
    {
        _cache = cache.ThrowIfNull();
        _httpClient = client.ThrowIfNull();
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
            () => PerformSearch(psQuery));

        if (!await results.MoveNextAsync())
            await FollowupAsync("No more results");
        else
            await FollowupAsync(results.Current.Url);
    }

    async IAsyncEnumerator<SearchResult> PerformSearch(PushshiftQuery baseQuery)
    {
        // mix 2 search results in together:
        // 1. ordered by highest score
        // 2. ordered by most recent
        // I think this is a good mixture of results to include
        var mostRecentTask = baseQuery
            .Clone()
            .Limit(SearchResultLimit)
            .Sort(SortType.CreatedDate, SortDirection.Descending)
            .Fields<PushshiftResult>()
            .Execute(_httpClient);

        var highestScoreTask = baseQuery
            .Clone()
            .Limit(SearchResultLimit)
            .Sort(SortType.Score, SortDirection.Descending)
            .Fields<PushshiftResult>()
            .Execute(_httpClient);

        await Task.WhenAll(mostRecentTask, highestScoreTask);

        var mostRecent = await mostRecentTask;
        var highestScore = await highestScoreTask;
        var combined = mostRecent
            .UnionBy(highestScore, x => x.Url)
            .Select(SearchResult.FromPushshift);

        var filtered = _filter.Filter(combined.ToAsyncEnumerable());

        await foreach (var item in filtered)
            yield return item;
    }
}