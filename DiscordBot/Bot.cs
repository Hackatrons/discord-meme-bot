﻿using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Commands;
using DiscordBot.Configuration;
using DiscordBot.Language;
using DiscordBot.Logging;
using Microsoft.Extensions.Options;

namespace DiscordBot;

internal class Bot : IAsyncDisposable
{
    readonly DiscordSocketClient _client;
    readonly CommandHandler _commandHandler;
    readonly InteractionService _interactions;
    readonly DiscordLogger _discordLogger;
    readonly DiscordSettings _settings;
    readonly IServiceProvider _provider;
    bool _disposed;

    public Bot(
        IServiceProvider serviceProvider,
        DiscordSocketClient client,
        CommandHandler commandHandler,
        InteractionService interactionService,
        DiscordLogger discordLogger,
        IOptions<DiscordSettings> discordSettings)
    {
        _provider = serviceProvider.ThrowIfNull();
        _client = client.ThrowIfNull();
        _commandHandler = commandHandler.ThrowIfNull();
        _interactions = interactionService.ThrowIfNull();
        _discordLogger = discordLogger.ThrowIfNull();
        _settings = discordSettings.ThrowIfNull().Value.ThrowIfNull();
    }

    public async Task StartAsync()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(Bot));

        _discordLogger.Initialise();
        _commandHandler.Initialise();

        // discover all of the commands in this assembly and load them.
        await _interactions.AddModulesAsync(typeof(PingCommand).Assembly, _provider);

        await _client.LoginAsync(TokenType.Bot, _settings.Token);
        await _client.StartAsync();
    }

    public async Task StopAsync()
    {
        _commandHandler.Dispose();

        await _client.LogoutAsync();
        await _client.StopAsync();
        await _client.DisposeAsync();

        _discordLogger.Dispose();

        _disposed = true;
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync();
    }
}