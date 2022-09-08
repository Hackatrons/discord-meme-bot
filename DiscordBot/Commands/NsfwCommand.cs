﻿using Discord.Interactions;
using DiscordBot.Caching;
using DiscordBot.Queries;
using JetBrains.Annotations;

namespace DiscordBot.Commands;

/// <summary>
/// A search command for NSFW content.
/// </summary>
[UsedImplicitly]
public class NsfwCommand : BaseSearchCommand
{
    public NsfwCommand(
        NsfwQueryHandler queryHandler,
        ICache cache)
        : base(queryHandler, cache) { }

    [UsedImplicitly]
    [SlashCommand("nsfw", "Search for only nsfw results.")]
    public async Task Execute(string query) => await Search(query);
}