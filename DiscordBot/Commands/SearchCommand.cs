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
    const int SearchResultLimit = 500;

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
        var initialResults = (await new PushshiftQuery()
            .Search(query)
            .Limit(SearchResultLimit)
            .Fields<PushshiftResult>()
            .Execute(_httpClient))
            .Select(SearchResult.FromPushshift);
        
        var filtered = _filter.Filter(initialResults.ToAsyncEnumerable());

        await foreach (var item in filtered)
            yield return item;
    }
}