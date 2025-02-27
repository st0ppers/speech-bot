﻿using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpeechDiscordBot.Commands;
using SpeechDiscordBot.Extensions;

namespace SpeechDiscordBot;

class Program
{
    public static async Task Main()
    {
        var services = DependencyInjection.ServiceProvider;
        var client = services.GetRequiredService<DiscordSocketClient>();
        var commandHandler = services.GetRequiredService<CommandHandler>();
        await commandHandler.InitializeAsync();

        await client.LoginAsync(TokenType.Bot, DependencyInjection.Configuration.GetRequiredSection("Discord:Token").Value);
        await client.StartAsync();

        await Task.Delay(-1);
    }
}