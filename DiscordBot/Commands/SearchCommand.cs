using Discord.Interactions;
using DiscordBot.Language;
using DiscordBot.Numerics;
using DiscordBot.Pushshift;
using JetBrains.Annotations;

namespace DiscordBot.Commands;

[UsedImplicitly]
public class SearchCommand : InteractionModuleBase<SocketInteractionContext>
{
    const int SearchResultLimit = 100;

    readonly HttpClient _httpClient;

    public SearchCommand(HttpClient client) => _httpClient = client.ThrowIfNull();

    [UsedImplicitly]
    [SlashCommand("search", "Perform a search for anything.")]
    public async Task Search(string query)
    {
        // we only get 3 seconds to respond before discord times out our request
        // but we can defer the response and have up to 15mins to provide a follow up response
        await DeferAsync();

        var results = (await new PushshiftQuery()
            .Search(query)
            .Limit(SearchResultLimit)
            .Execute(_httpClient))
            .ToList();

        var randomResult = results[ThreadSafeRandom.Random.Next(0, results.Count -1)];

        await FollowupAsync(randomResult.Url);
    }
}