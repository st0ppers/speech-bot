using Discord.Commands;
using Microsoft.Extensions.Configuration;
using SpeachDiscordBot.Configuration;

namespace SpeachDiscordBot.Commands.Loldle;

public class LoldleGameModule : ModuleBase<SocketCommandContext>
{
    private readonly HttpClient _httpClient = new();

    [Command("test", RunMode = RunMode.Async)]
    public async Task TestAsync()
    {
        _httpClient.BaseAddress = new Uri(
            Extensions.Configuration.GetRequiredSection("LeagueOfLegendsApi:BaseUrl").Value +
            Extensions.Configuration.GetRequiredSection("LeagueOfLegendsApi:Patch").Value +
            Extensions.Configuration.GetRequiredSection("LeagueOfLegendsApi:Format").Value +
            Extensions.Configuration.GetRequiredSection("LeagueOfLegendsApi:Language").Value
        );
        var endpoint = Extensions.Configuration.GetRequiredSection("LeagueOfLegendsApi:Endpoint").Value;
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