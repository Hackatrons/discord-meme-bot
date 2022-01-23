using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Configuration;
using DiscordBot.Language;

namespace DiscordBot.Commands;

internal class CommandHandler : IDisposable
{
    readonly DiscordSocketClient _client;
    readonly InteractionService _interactions;
    readonly IServiceProvider _provider;

    public CommandHandler(
        DiscordSocketClient client,
        InteractionService interactions,
        IServiceProvider serviceProvider)
    {
        _client = client.ThrowIfNull();
        _interactions = interactions.ThrowIfNull();
        _provider = serviceProvider.ThrowIfNull();
    }

    public void Initialise()
    {
        _client.Ready += OnReady;
        _client.GuildAvailable += OnGuildAvailable;
        _client.InteractionCreated += OnInteractionCreated;
        _interactions.SlashCommandExecuted += OnSlashCommandExecuted;
    }

    public void Dispose()
    {
        _client.Ready -= OnReady;
        _client.GuildAvailable -= OnGuildAvailable;
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
        var message = new EmbedBuilder()
            .WithColor(BotColours.Error)
            .AddField(x =>
            {
                x.Name = error.Error switch
                {
                    InteractionCommandError.BadArgs
                        or InteractionCommandError.ParseFailed
                        or InteractionCommandError.ConvertFailed => "Invalid Arguments",
                    InteractionCommandError.UnknownCommand => "Unknown Command",
                    _ => "Error"
                };
                x.Value = error.ErrorReason ?? "An unknown error has occurred.";
            })
            .Build();

        if (!context.Interaction.HasResponded)
            await context.Interaction.RespondAsync(embed: message);
        else
            await context.Interaction.FollowupAsync(embed: message);
    }

    async Task OnGuildAvailable(SocketGuild arg)
    {
#if DEBUG
        await _interactions.RegisterCommandsToGuildAsync(arg.Id);
#endif
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    // ReSharper disable once MemberCanBeMadeStatic.Local
    async Task OnReady()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
#if !DEBUG
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