using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using static SpeachDiscordBot.Configuration.Extensions;

namespace SpeachDiscordBot;

class Program
{
    private static readonly IConfiguration Configuration = AddConfiguration();

    private static readonly ILogger Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .CreateLogger();

    private static readonly DiscordSocketClient Client = new(new DiscordSocketConfig
    {
        MessageCacheSize = 100,
        LogLevel = LogSeverity.Debug,
        GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
    });

    private static readonly CommandService CommandService = new(new CommandServiceConfig
    {
        LogLevel = LogSeverity.Info,
        CaseSensitiveCommands = false
    });

    static async Task Main(string[] args)
    {
        Client.Log += LogAsync;
        CommandService.Log += LogAsync;

        await Client.LoginAsync(TokenType.Bot, Configuration.GetRequiredSection("Token").Value);
        await Client.StartAsync();

        await InitialCommandASync();
        await Task.Delay(-1);
    }

    //Maybe move commands to separete file?
    private static async Task InitialCommandASync()
    {
        Client.MessageReceived += HandleCommandAsync;

        await CommandService.AddModulesAsync(Assembly.GetEntryAssembly(), null);
    }

    private static async Task HandleCommandAsync(SocketMessage arg)
    {
        var msg = arg as SocketUserMessage;

        if (msg is null)
        {
            return;
        }

        var argPos = 0;
        if (!msg.HasStringPrefix(Configuration.GetRequiredSection("Prefix").Value, ref argPos) || msg.HasMentionPrefix(Client.CurrentUser, ref argPos) || msg.Author.IsBot)
        {
            return;
        }

        var context = new SocketCommandContext(Client, msg);

        await CommandService.ExecuteAsync(context, argPos, null);
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