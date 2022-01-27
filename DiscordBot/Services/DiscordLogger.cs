using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Language;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Services;

internal class DiscordLogger : IInitialise
{
    readonly DiscordSocketClient _client;
    readonly InteractionService _interactions;
    readonly ILogger _logger;

    public DiscordLogger(
        DiscordSocketClient client,
        InteractionService interactions,
        ILogger<DiscordLogger> logger)
    {
        _client = client.ThrowIfNull();
        _interactions = interactions.ThrowIfNull();
        _logger = logger.ThrowIfNull();
    }

    public void Initialise()
    {
        _client.Log += OnLog;
        _interactions.Log += OnLog;
        _interactions.SlashCommandExecuted += OnSlashCommandExecuted;
    }

    Task OnSlashCommandExecuted(
        SlashCommandInfo command,
        IInteractionContext context,
        IResult result)
    {
        var parameters = context.Interaction.Data as SocketSlashCommandData;
        var parametersMessage = string.Join(",", parameters?.Options.Select(x => x.Name + "=" + x.Value) ?? Array.Empty<string>());

        if (result.IsSuccess)
        {
            _logger.LogInformation(
                "Executed command '{name}' with parameters '{params}' for user '{user}'",
                command.Name,
                parametersMessage,
                context.User.Username);
        }
        else
        {
            _logger.LogError(
                "Error while executing command '{name}' with parameters '{params}' for user '{user}'. Error Code: '{code}', Reason: '{reason}'",
                // ReSharper disable once ConstantConditionalAccessQualifier
                // command object is null if it's an unknown command
                command?.Name ?? "Unknown",
                parametersMessage,
                context.User.Username,
                result.Error?.ToString(),
                result.ErrorReason);
        }

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _client.Log -= OnLog;
        _interactions.Log -= OnLog;
        _interactions.SlashCommandExecuted -= OnSlashCommandExecuted;
    }

    Task OnLog(LogMessage arg)
    {
        // ignore the warning about static templates
        // as we aren't using a message template here, just forwarding on a static log message
        // as opposed to a structured log message which is what this warning is about
#pragma warning disable CA2254 // Template should be a static expression
        _logger.Log(SeverityToLevel(arg.Severity), arg.Exception, arg.Message);
#pragma warning restore CA2254 // Template should be a static expression

        return Task.CompletedTask;
    }

    static LogLevel SeverityToLevel(LogSeverity level) =>
        level switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Debug => LogLevel.Debug,
            LogSeverity.Verbose => LogLevel.Trace,
            _ => LogLevel.Information
        };
}