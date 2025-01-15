using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace SpeachDiscordBot.Configuration;

public static class Extensions
{
    private const string DefaultConfigFileName = "appsettings.json";

    public static IConfiguration AddConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(DefaultConfigFileName, false);
        return builder.Build();
    }
}