using System.Reflection;
using System.Runtime.InteropServices;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using SpeechDiscordBot.Configuration;

namespace SpeechDiscordBot.Commands;

public class CommandHandler(IOptions<DiscordConfigruation> config, IServiceProvider services, CommandService commands, DiscordSocketClient client, ILogger logger)
{
    public async Task InitializeAsync()
    {
        Log.Logger.Error("Operating System: {OSDescription}",RuntimeInformation.OSDescription);
        InitialLogger();
        await InitialCommandASync();
    }

    private async Task InitialCommandASync()
    {
        client.MessageReceived += HandleCommandAsync;
        await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);
    }

    private void InitialLogger()
    {
        client.Log += LogAsync;
        commands.Log += LogAsync;
    }

    private async Task HandleCommandAsync(SocketMessage arg)
    {
        var msg = arg as SocketUserMessage;

        if (msg is null)
        {
            return;
        }

        var argPos = 0;
        if (!msg.HasStringPrefix(config.Value.Prefix, ref argPos) || msg.HasMentionPrefix(client.CurrentUser, ref argPos) || msg.Author.IsBot)
        {
            return;
        }

        var context = new SocketCommandContext(client, msg);

        await commands.ExecuteAsync(context, argPos, services);
    }

    private Task LogAsync(LogMessage message)
    {
        var severity = message.Severity switch
        {
            LogSeverity.Critical => LogEventLevel.Fatal,
            LogSeverity.Error => LogEventLevel.Error,
            LogSeverity.Warning => LogEventLevel.Warning,
            LogSeverity.Info => LogEventLevel.Information,
            LogSeverity.Debug => LogEventLevel.Debug,
            _ => LogEventLevel.Information
        };

        logger.Write(severity, message.Exception, "[{Source}] {Message}", message.Source, message.Message);
        return Task.CompletedTask;
    }
}