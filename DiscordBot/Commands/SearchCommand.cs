using Discord.Interactions;
using DiscordBot.Caching;
using DiscordBot.Filters;
using DiscordBot.Language;
using DiscordBot.Models;
using DiscordBot.Pushshift;
using DiscordBot.Pushshift.Models;
using JetBrains.Annotations;

namespace DiscordBot.Commands;

[UsedImplicitly]
// commands must be public classes for discord.net to use them
public class SearchCommand : InteractionModuleBase<SocketInteractionContext>
{
    const int SearchResultLimit = 200;

    readonly ResultsCache _cache;
    readonly HttpClient _httpClient;
    readonly AggregateFilter _filter;

    public SearchCommand(
        ResultsCache cache,
        AggregateFilter filter,
        HttpClient client)
    {
        _cache = cache.ThrowIfNull();
        _httpClient = client.ThrowIfNull();
        _filter = filter.ThrowIfNull();
    }

    [UsedImplicitly]
    [SlashCommand("search", "Perform a search for anything.")]
    public async Task Search(string query)
    {
        // we only get 3 seconds to respond before discord times out our request
        // but we can defer the response and have up to 15mins to provide a follow up response
        await DeferAsync();

        var results = _cache.GetOrAdd(
            Context.Channel.Id,
            nameof(SearchCommand),
            query,
            () => PerformSearch(query));

        if (!await results.MoveNextAsync())
            await FollowupAsync("No more results");
        else
            await FollowupAsync(results.Current.Url);
    }

    async IAsyncEnumerator<SearchResult> PerformSearch(string query)
    {
        // mix 2 search results in together:
        // 1. ordered by highest score
        // 2. ordered by most recent
        // I think this is a good mixture of results to include
        var mostRecentTask = new PushshiftQuery()
            .Search(query)
            .Limit(SearchResultLimit)
            .Sort(SortType.CreatedDate, SortDirection.Descending)
            .Fields<PushshiftResult>()
            .Execute(_httpClient);

        var highestScoreTask = new PushshiftQuery()
            .Search(query)
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