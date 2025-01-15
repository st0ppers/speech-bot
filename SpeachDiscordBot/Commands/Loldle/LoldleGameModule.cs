using System.Runtime.InteropServices.JavaScript;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using static SpeachDiscordBot.Configuration.Extensions;

namespace SpeachDiscordBot.Commands.Loldle;

public class LoldleGameModule : ModuleBase<SocketCommandContext>
{
    private readonly HttpClient _httpClient = new();
    private readonly IConfiguration _configuration = AddConfiguration();

    [Command("test", RunMode = RunMode.Async)]
    public async Task TestAsync()
    {
        _httpClient.BaseAddress = new Uri(
            _configuration.GetRequiredSection("LeagueOfLegendsApi:BaseUrl").Value +
            _configuration.GetRequiredSection("LeagueOfLegendsApi:Patch").Value +
            _configuration.GetRequiredSection("LeagueOfLegendsApi:Format").Value +
            _configuration.GetRequiredSection("LeagueOfLegendsApi:Language").Value
        );
        var endpoint = _configuration.GetRequiredSection("LeagueOfLegendsApi:Endpoint").Value;
        var apiKey = _configuration.GetRequiredSection("LeagueOfLegendsApi:Key").Value;
        try
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
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