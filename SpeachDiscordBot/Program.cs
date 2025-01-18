using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpeachDiscordBot.Commands;
using SpeachDiscordBot.Extensions;

class Program
{
    static async Task Main(string[] args)
    {
        var services = DependencyInjection.ServiceProvider;
        var client = services.GetRequiredService<DiscordSocketClient>();
        var commandHandler = services.GetRequiredService<CommandHandler>();
        await commandHandler.InitializeAsync();
        
        await client.LoginAsync(TokenType.Bot, DependencyInjection.Configuration.GetRequiredSection("Token").Value);
        await client.StartAsync();

        await Task.Delay(-1);
    }
}