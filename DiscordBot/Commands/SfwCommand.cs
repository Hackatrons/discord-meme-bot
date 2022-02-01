using Discord.Interactions;
using DiscordBot.Caching;
using DiscordBot.Filters;
using DiscordBot.Pushshift;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Commands;

/// <summary>
/// A search command for SFW content.
/// </summary>
[UsedImplicitly]
public class SfwCommand : BaseSearchCommand
{
    public SfwCommand(
        ResultsCache cache,
        AggregateFilter filter,
        IHttpClientFactory httpClientFactory,
        RepeatCommandCache repeatCommandHandler,
        ILogger<SfwCommand> logger) : base(cache, filter, httpClientFactory, repeatCommandHandler, logger) { }

    [UsedImplicitly]
    [SlashCommand("sfw", "Search for only sfw results.")]
    public Task Execute(string query) =>
        ExecuteInternal(query);

    protected override PushshiftQuery BuildBaseQuery(string query) =>
        new PushshiftQuery()
            .Search(query)
            .Sfw();
}