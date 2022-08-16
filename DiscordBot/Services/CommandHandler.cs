using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Configuration;
using DiscordBot.Language;
using DiscordBot.Messaging;
using DiscordBot.Threading;

namespace DiscordBot.Services;

/// <summary>
/// Handler for executing commands.
/// </summary>
internal class CommandHandler : IInitialise
{
    readonly DiscordSocketClient _client;
    readonly InteractionService _interactions;
    readonly IServiceProvider _provider;
    readonly TestingSettings _testingSettings;

    public CommandHandler(
        DiscordSocketClient client,
        InteractionService interactions,
        IServiceProvider serviceProvider,
        TestingSettings testingSettings)
    {
        _client = client.ThrowIfNull();
        _interactions = interactions.ThrowIfNull();
        _provider = serviceProvider.ThrowIfNull();
        _testingSettings = testingSettings.ThrowIfNull();
    }

    public void Initialise()
    {
        _client.Ready += OnReady;
        _client.InteractionCreated += OnInteractionCreated;
        _interactions.SlashCommandExecuted += OnSlashCommandExecuted;
    }

    public void Dispose()
    {
        _client.Ready -= OnReady;
        _client.InteractionCreated -= OnInteractionCreated;
        _interactions.SlashCommandExecuted -= OnSlashCommandExecuted;
    }

    static Task OnSlashCommandExecuted(SlashCommandInfo command, IInteractionContext context, IResult result)
    {
        Task.Run(async () =>
        {
            if (!result.IsSuccess)
                await RespondError(context, result);
        }).Forget();

        return Task.CompletedTask;
    }

    static async Task RespondError(IInteractionContext context, IResult error)
    {
        var message = BotMessage.Error(error.Error switch
        {
            InteractionCommandError.BadArgs
                or InteractionCommandError.ParseFailed
                or InteractionCommandError.ConvertFailed => "Invalid Arguments",
            InteractionCommandError.UnknownCommand => "Unknown Command",
            _ => "Error"
        }, error.ErrorReason ?? "An unknown error has occurred.");

        if (!context.Interaction.HasResponded)
            await context.Interaction.RespondAsync(embed: message);
        else
            await context.Interaction.FollowupAsync(embed: message);
    }

    async Task OnReady()
    {
#if DEBUG
        if (_testingSettings.TestServerId != null &&
            _client.Guilds.Any(x => x.Id == _testingSettings.TestServerId))
        {
            await _interactions.RegisterCommandsToGuildAsync(_testingSettings.TestServerId.Value);
        }
#else
        await _interactions.RegisterCommandsGloballyAsync();
#endif
    }

    Task OnInteractionCreated(SocketInteraction arg)
    {
        // run in a background thread to avoid blocking the discord.net gateway task
        Task.Run(async () =>
        {
            var context = new SocketInteractionContext(_client, arg);
            var result = await _interactions.ExecuteCommandAsync(context, _provider);

            if (!result.IsSuccess)
                await RespondError(context, result);
        }).Forget();

        return Task.CompletedTask;
    }
}