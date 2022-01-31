using Discord.Interactions;
using DiscordBot.Caching;
using DiscordBot.Filters;
using DiscordBot.Pushshift;
using JetBrains.Annotations;

namespace DiscordBot.Commands;

/// <summary>
/// A search command for NSFW content.
/// </summary>
[UsedImplicitly]
public class NsfwCommand : BaseSearchCommand
{
    public NsfwCommand(
        ResultsCache cache,
        AggregateFilter filter,
        IHttpClientFactory httpClientFactory,
        RepeatCommandCache repeatCommandHandler) : base(cache, filter, httpClientFactory, repeatCommandHandler) { }

    [UsedImplicitly]
    [SlashCommand("nsfw", "Search for only nsfw results.")]
    public Task Execute(string query) => Search(query);

    protected override PushshiftQuery BuildBaseQuery(string query) =>
        new PushshiftQuery()
            .Search(query)
            .Nsfw();
}