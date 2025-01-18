using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpeachDiscordBot.Commands;

namespace SpeachDiscordBot.Configuration;

public static class Extensions
{
    private const string DefaultConfigFileName = "appsettings.json";

    public static IConfiguration Configuration =>
        new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(DefaultConfigFileName, false)
            .Build();

    public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<ElevenLabsConfiguration>().Bind(configuration.GetRequiredSection(ElevenLabsConfiguration.Section));
        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<Voice>();
        return services;
    }

    // public static ServiceProvider CreateServices()
    public static ServiceProvider ServiceProvider =>
        new ServiceCollection()
            .AddConfiguration(Configuration)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton<CommandService>()
            .AddSingleton<CommandHandler>()
            .AddServices()
            .AddHttpClient()
            .BuildServiceProvider();

    // return serviceProvider;
    // }
}