using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpeachDiscordBot.Client;
using SpeachDiscordBot.Commands;
using SpeachDiscordBot.Configuration;

namespace SpeachDiscordBot.Extensions;

public static class DependencyInjection
{
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
            //.AddLogging()
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