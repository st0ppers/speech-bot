using Discord.Commands;
using Microsoft.Extensions.Configuration;
using SpeechDiscordBot.Extensions;

namespace SpeechDiscordBot.Commands.Loldle;

public class LoldleGameModule : ModuleBase<SocketCommandContext>
{
    private readonly HttpClient _httpClient = new();

    [Command("test", RunMode = RunMode.Async)]
    public async Task TestAsync()
    {
        _httpClient.BaseAddress = new Uri(
            DependencyInjection.Configuration.GetRequiredSection("LeagueOfLegendsApi:BaseUrl").Value +
            DependencyInjection.Configuration.GetRequiredSection("LeagueOfLegendsApi:Patch").Value +
            DependencyInjection.Configuration.GetRequiredSection("LeagueOfLegendsApi:Format").Value +
            DependencyInjection.Configuration.GetRequiredSection("LeagueOfLegendsApi:Language").Value
        );
        var endpoint = DependencyInjection.Configuration.GetRequiredSection("LeagueOfLegendsApi:Endpoint").Value;
        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                var r = await response.Content.ReadAsStringAsync();
            }
            else
            {
                var a = await response.Content.ReadAsStringAsync();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}