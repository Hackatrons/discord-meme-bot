using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Commands;
using DiscordBot.Configuration;
using DiscordBot.Language;
using DiscordBot.Services;
using Microsoft.Extensions.Options;

namespace DiscordBot;

/// <summary>
/// Starts and stops the discord bot.
/// </summary>
internal class Bot : IAsyncDisposable
{
    readonly IInitialise[] _services;
    readonly DiscordSocketClient _client;
    readonly InteractionService _interactions;
    readonly DiscordSettings _settings;
    readonly IServiceProvider _provider;
    bool _disposed;

    public Bot(
        IServiceProvider serviceProvider,
        IInitialise[] services,
        DiscordSocketClient client,
        InteractionService interactionService,
        IOptions<DiscordSettings> discordSettings)
    {
        _services = services.ThrowIfNull();
        _provider = serviceProvider.ThrowIfNull();
        _client = client.ThrowIfNull();
        _interactions = interactionService.ThrowIfNull();
        _settings = discordSettings.ThrowIfNull().Value.ThrowIfNull();
    }

    /// <summary>
    /// Starts the bot and returns once started.
    /// </summary>
    public async Task StartAsync()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(Bot));

        foreach (var service in _services)
            service.Initialise();

        // discover all of the commands in this assembly and load them.
        await _interactions.AddModulesAsync(typeof(SearchCommand).Assembly, _provider);

        await _client.LoginAsync(TokenType.Bot, _settings.Token);
        await _client.StartAsync();
    }

    /// <summary>
    /// Stops and disposes the bot.
    /// </summary>
    public async Task StopAsync()
    {
        foreach (var service in _services)
            service.Dispose();

        await _client.LogoutAsync();
        await _client.StopAsync();
        await _client.DisposeAsync();

        _disposed = true;
    }

    /// <summary>
    /// Disposes the bot.
    /// </summary>
    public async ValueTask DisposeAsync() => await StopAsync();
}