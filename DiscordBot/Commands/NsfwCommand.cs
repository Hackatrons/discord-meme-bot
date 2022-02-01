using Discord.Interactions;
using DiscordBot.Caching;
using DiscordBot.Filters;
using DiscordBot.Pushshift;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

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
        RepeatCommandCache repeatCommandHandler,
        ILogger<NsfwCommand> logger) : base(cache, filter, httpClientFactory, repeatCommandHandler, logger) { }

    [UsedImplicitly]
    [SlashCommand("nsfw", "Search for only nsfw results.")]
    public Task Execute(string query) => ExecuteInternal(query);

    protected override PushshiftQuery BuildBaseQuery(string query) =>
        new PushshiftQuery()
            .Search(query)
            .Nsfw();
}