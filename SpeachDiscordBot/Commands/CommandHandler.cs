using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using SpeachDiscordBot.Configuration;

namespace SpeachDiscordBot.Commands;

public class CommandHandler(IServiceProvider services, CommandService commands, DiscordSocketClient client)
{
    private static readonly ILogger Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .CreateLogger();

    public async Task InitializeAsync()
    {
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
        if (!msg.HasStringPrefix(Extensions.Configuration.GetRequiredSection("Prefix").Value, ref argPos) || msg.HasMentionPrefix(client.CurrentUser, ref argPos) || msg.Author.IsBot)
        {
            return;
        }

        var context = new SocketCommandContext(client, msg);

        await commands.ExecuteAsync(context, argPos, services);
    }

    private static Task LogAsync(LogMessage message)
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

        Logger.Write(severity, message.Exception, "[{Source}] {Message}", message.Source, message.Message);
        return Task.CompletedTask;
    }
}