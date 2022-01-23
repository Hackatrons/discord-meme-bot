using Discord;
using Discord.WebSocket;
using DiscordBot.Language;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Logging
{
    internal class DiscordSocketClientLogger
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        readonly DiscordSocketClient _client;
        readonly ILogger _log;

        public DiscordSocketClientLogger(DiscordSocketClient client, ILogger<DiscordSocketClientLogger> log)
        {
            _client = client.ThrowIfNull(nameof(client));
            _log = log.ThrowIfNull(nameof(log));

            _client.Log += OnLog;
        }

        Task OnLog(LogMessage arg)
        {
            _log.Log(SeverityToLevel(arg.Severity), arg.Exception, arg.Message);

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
                _ => LogLevel.Information,
            };
    }
}
