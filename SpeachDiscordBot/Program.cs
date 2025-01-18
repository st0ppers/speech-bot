using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpeachDiscordBot.Commands;
using SpeachDiscordBot.Configuration;

namespace SpeachDiscordBot;

class Program
{
    static async Task Main(string[] args)
    {
        var client = Extensions.ServiceProvider.GetRequiredService<DiscordSocketClient>();
        var commandHandler = Extensions.ServiceProvider.GetRequiredService<CommandHandler>();
        await commandHandler.InitializeAsync();
        
        await client.LoginAsync(TokenType.Bot, Extensions.Configuration.GetRequiredSection("Token").Value);
        await client.StartAsync();

        await Task.Delay(-1);
    }
}