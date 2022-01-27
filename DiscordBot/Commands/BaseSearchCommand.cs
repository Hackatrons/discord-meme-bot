using Discord;
using Discord.Interactions;
using DiscordBot.Caching;
using DiscordBot.Filters;
using DiscordBot.Language;
using DiscordBot.Models;
using DiscordBot.Pushshift;
using DiscordBot.Pushshift.Models;
using DiscordBot.Reactions;
using DiscordBot.Threading;

namespace DiscordBot.Commands;

// commands must be public classes for discord.net to use them
public abstract class BaseSearchCommand : InteractionModuleBase<SocketInteractionContext>
{
    const int SearchResultLimit = 200;

    static readonly IEmote[] ResultEmotes =
    {
        Emotes.Delete,
        Emotes.ThumbsUp,
        Emotes.ThumbsDown,
        Emotes.Repeat
    };

    readonly ResultsCache _cache;
    readonly RepeatCommandCache _repeatCommandHandler;
    readonly IHttpClientFactory _httpClientFactory;
    readonly AggregateFilter _filter;
    readonly string _typeName;

    protected BaseSearchCommand(
        ResultsCache cache,
        AggregateFilter filter,
        IHttpClientFactory httpClientFactory,
        RepeatCommandCache repeatCommandHandler)
    {
        _cache = cache.ThrowIfNull();
        _filter = filter.ThrowIfNull();
        _httpClientFactory = httpClientFactory.ThrowIfNull();
        _repeatCommandHandler = repeatCommandHandler.ThrowIfNull();
        _typeName = GetType().Name;
    }

    protected abstract PushshiftQuery BuildBaseQuery(string query);

    protected async Task Search(string query)
    {
        // we only get 3 seconds to respond before discord times out our request
        // but we can defer the response and have up to 15mins to provide a follow up response
        await DeferAsync();

        var result = await GetNextResult(query);
        if (result == null)
        {
            await FollowupAsync("No results.");
            return;
        }

        var message = await FollowupAsync(result.Url);
        AddReactionsAndWatch(message, query);
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

        var mostRecent = await mostRecentTask;
        var highestScore = await highestScoreTask;
        var combined = mostRecent
            .UnionBy(highestScore, x => x.Url)
            .Select(SearchResult.FromPushshift);

        return combined;
    }

    async Task<SearchResult?> GetNextResult(string query)
    {
        var psQuery = BuildBaseQuery(query);

        var results = _cache.GetOrAdd(
            Context.Channel.Id,
            _typeName,
            query,
            () => Search(psQuery));

        if (!await results.MoveNextAsync())
            return null;

        return results.Current;
    }

    void AddReactionsAndWatch(IUserMessage message, string query)
    {
        _repeatCommandHandler.Add(message.Id, () => RepeatCommand(query));
        // adding reactions is very slow, so do this in a background task
        message.AddReactionsAsync(ResultEmotes).Forget();
    }

    async Task RepeatCommand(string query)
    {
        using var state = Context.Channel.EnterTypingState();
        var result = await GetNextResult(query);
        if (result == null)
        {
            await ReplyAsync("No results.");
            return;
        }

        var message = await ReplyAsync(result.Url);
        AddReactionsAndWatch(message, query);
    }
}