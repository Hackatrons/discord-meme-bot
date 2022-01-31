using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Configuration;
using DiscordBot.Language;
using DiscordBot.Messaging;
using Microsoft.Extensions.Options;

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
        IOptions<TestingSettings> testingSettings)
    {
        _testingSettings = testingSettings.Value;
        _client = client.ThrowIfNull();
        _interactions = interactions.ThrowIfNull();
        _provider = serviceProvider.ThrowIfNull();
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

    static async Task OnSlashCommandExecuted(SlashCommandInfo command, IInteractionContext context, IResult result)
    {
        if (!result.IsSuccess)
            await RespondError(context, result);
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

    async Task OnInteractionCreated(SocketInteraction arg)
    {
        var context = new SocketInteractionContext(_client, arg);
        var result = await _interactions.ExecuteCommandAsync(context, _provider);

        if (!result.IsSuccess)
            await RespondError(context, result);
    }
}