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
using Microsoft.Extensions.Logging;

namespace DiscordBot.Commands;

/// <summary>
/// Base class for search related commands.
/// Note: commands must be public classes for discord.net to use them
/// </summary>
public abstract class BaseSearchCommand : InteractionModuleBase<SocketInteractionContext>
{
    // pushshift has a hard limit of 100
    const int SearchResultLimit = 100;
    const string NoResultsMessage = "No results.";

    static readonly IEmote[] ResultEmotes =
    {
        Emotes.Delete,
        Emotes.Repeat
    };

    readonly ResultsCache _cache;
    readonly RepeatCommandCache _repeatCommandCache;
    readonly IHttpClientFactory _httpClientFactory;
    readonly AggregateFilter _filter;
    readonly string _typeName;
    readonly ILogger _logger;

    protected BaseSearchCommand(
        ResultsCache cache,
        AggregateFilter filter,
        IHttpClientFactory httpClientFactory,
        RepeatCommandCache repeatCommandHandler,
        ILogger logger)
    {
        _cache = cache.ThrowIfNull();
        _filter = filter.ThrowIfNull();
        _httpClientFactory = httpClientFactory.ThrowIfNull();
        _repeatCommandCache = repeatCommandHandler.ThrowIfNull();
        _typeName = GetType().Name;
        _logger = logger.ThrowIfNull();
    }

    /// <summary>
    /// Builds a base pushshift query for the given search string.
    /// </summary>
    protected abstract PushshiftQuery BuildBaseQuery(string query);

    protected async Task ExecuteInternal(string query)
    {
        // we only get 3 seconds to respond before discord times out our request
        // but we can defer the response and have up to 15mins to provide a follow up response
        await DeferAsync();

        // retrieve the next result (either from cache or executing the query)
        var result = await GetNextResult(query);
        if (result == null)
        {
            await FollowupAsync(NoResultsMessage);
            return;
        }

        var message = await FollowupAsync(result.Url);
        AddReactionsAndWatch(message, query);
    }

    async Task<SearchResult?> GetNextResult(string query)
    {
        var results = _cache.GetOrAdd(
            Context.Channel.Id,
            _typeName,
            query,
            () => Search(query));

        return await results.MoveNextAsync() ? results.Current : null;
    }

    async IAsyncEnumerator<SearchResult> Search(string query)
    {
        var results = await GetResults(query);
        var filtered = _filter.Filter(results.ToAsyncEnumerable());

        await foreach (var item in filtered)
            yield return item;
    }

    async Task<IEnumerable<SearchResult>> GetResults(string query)
    {
        using var httpClient = _httpClientFactory.CreateClient();

        var baseQuery = BuildBaseQuery(query);

        // mix 2 search results in together:
        // 1. ordered by highest score
        // 2. ordered by most recent
        // probably a good mixture of results
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

        var mostRecent = (await mostRecentTask).ToList();
        var highestScore = (await highestScoreTask).ToList();

        // merge the result sets
        var combined = mostRecent
            .UnionBy(highestScore, x => x.Url)
            .ToList();

        // extract any additional results from the reddit post
        var additionalResults = combined
            .SelectMany(x => x.ExtractUrls())
            .Select(x => new SearchResult { Url = x })
            .ToList();

        // merge the additional results in
        // the final result be unique by url
        var results = combined
            .Select(SearchResult.FromPushshift)
            .UnionBy(additionalResults, x => x.Url)
            .ToList();

        _logger.LogDebug("Found {count} most recent results for query '{query}'.", mostRecent.Count, query);
        _logger.LogDebug("Found {count} highest score results for query '{query}'.", highestScore.Count, query);
        _logger.LogDebug("Found {count} additional urls for query '{query}'.", additionalResults.Count, query);
        _logger.LogDebug("After merging, a total of {count} results were found for query '{query}'.", results.Count, query);

        return results;
    }

    async Task Repeat(string query)
    {
        using var state = Context.Channel.EnterTypingState();
        var result = await GetNextResult(query);
        if (result == null)
        {
            await ReplyAsync(NoResultsMessage);
            return;
        }

        var message = await ReplyAsync(result.Url);
        AddReactionsAndWatch(message, query);
    }

    void AddReactionsAndWatch(IUserMessage message, string query)
    {
        // add the repeat action
        _repeatCommandCache.Add(message.Id, () => Repeat(query));

        // adding reactions is very slow, so do this in a background task
        message.AddReactionsAsync(ResultEmotes).Forget();
    }
}