﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SpeechDiscordBot.Client;
using SpeechDiscordBot.Commands;
using SpeechDiscordBot.Configuration;

namespace SpeechDiscordBot.Extensions;

public static class DependencyInjection
{
    private static readonly ILogger Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .CreateLogger();
    
    private static readonly DiscordSocketConfig DiscordSocketConfig = new()
    {
        MessageCacheSize = 100,
        LogLevel = LogSeverity.Debug,
        GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
    };

    private static readonly CommandServiceConfig CommandServiceConfig = new()
    {
        LogLevel = LogSeverity.Info,
        CaseSensitiveCommands = false
    };

    private const string DefaultConfigFileName = "appsettings.json";

    private static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<ElevenLabsConfiguration>().Bind(configuration.GetRequiredSection(ElevenLabsConfiguration.Section));
        services.AddOptions<DiscordConfigruation>().Bind(configuration.GetRequiredSection(DiscordConfigruation.Section));
        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services.AddSingleton<Voice>()
            .AddSingleton(DiscordSocketConfig)
            .AddSingleton(CommandServiceConfig)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton<CommandService>()
            .AddSingleton<CommandHandler>()
            .AddSingleton<ElevenLabsClient>()
            .AddSingleton(Logger)
            .AddHttpClient();
    }

    public static IConfiguration Configuration =>
        new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(DefaultConfigFileName, false)
            .Build();

    public static ServiceProvider ServiceProvider =>
        new ServiceCollection()
            .AddConfiguration(Configuration)
            .AddServices()
            .BuildServiceProvider();
}
