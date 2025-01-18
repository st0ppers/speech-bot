using System.Diagnostics;
using System.Text;
using Discord;
using Discord.Audio;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using SpeachDiscordBot.Configuration;

namespace SpeachDiscordBot.Commands;

public class Voice(IOptions<ElevenLabsConfiguration> config) : ModuleBase<SocketCommandContext>
{
    private readonly HttpClient _httpClient = new();

    [Command("join", RunMode = RunMode.Async)]
    public async Task JoinChannel(IVoiceChannel? channel = null)
    {
        try
        {
            channel ??= (Context.User as IGuildUser)?.VoiceChannel;
            if (channel == null)
            {
                return;
            }

            var audioClient = await channel.ConnectAsync(true);
            await SendAsync(audioClient, "C:/Users/Coding/Projects/SpeachDiscordBot/SpeachDiscordBot/fart.mp3");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [Command("say", RunMode = RunMode.Async)]
    // public async Task SayPhrase(IVoiceChannel? channel = null, params string[] phrases)
    public async Task SayPhrase(params string[] text)
    {
        //if text.trim() exists don't send a call
        //just use the one in the system
        //Every 5 min delete any .mp3 files
        try
        {
            var serialized = JsonConvert.SerializeObject(new { text = text[0] });

            // const string voiceId = "xZp4zaaBzoWhWxxrcAij";
            // _httpClient.BaseAddress = new Uri($"{baseUrl}{voiceId}?{optionOne}={ValueOne}&{OptionTwo}={ValueTwo}");
            // _httpClient.DefaultRequestHeaders.Add("xi-api-key", _configuration.GetRequiredSection("ElevenLabs:Key").Value);
            // var content = new StringContent(serialized, Encoding.UTF8, "application/json");
            // var response = await _httpClient.PostAsync("", content);
            
            //TODO Not tested the new version if it works (Refactor with caching, look for flie in the directory if it exists use it, so we don't create new request)
            var st = ToParameters(config.Value.Options);
            _httpClient.BaseAddress = new Uri($"{config.Value.BaseUrl}{config.Value.OldManId}?{st}");
            _httpClient.DefaultRequestHeaders.Add("xi-api-key", config.Value.Key);
            var content = new StringContent(serialized, Encoding.UTF8, config.Value.MediaType);
            var response = await _httpClient.PostAsync(string.Empty, content);

            if (response.IsSuccessStatusCode)
            {
                //Здравей наско, как си прекарваш тази приятна вечер
                var bytes = await response.Content.ReadAsByteArrayAsync();
                //Test this with run mode
                var filePath = $"{Directory.GetCurrentDirectory()}/{text[0].Replace(" ", string.Empty)}.mp3";
                await File.WriteAllBytesAsync(filePath, bytes);
                Log.Logger.Debug("File has been created.");
            }

            var channel = (Context.User as IGuildUser)?.VoiceChannel;
            if (channel == null)
            {
                return;
            }

            var audioClient = await channel.ConnectAsync(true);
            await SendAsync(audioClient, $"{Directory.GetCurrentDirectory()}/{text[0].Replace(" ", string.Empty)}.mp3");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private Process? CreateStream(string path)
    {
        return Process.Start(new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
            UseShellExecute = false,
            RedirectStandardOutput = true,
        });
    }

    private async Task SendAsync(IAudioClient client, string path)
    {
        using var ffmpeg = CreateStream(path);
        await using var output = ffmpeg?.StandardOutput.BaseStream;
        await using var discord = client.CreatePCMStream(AudioApplication.Mixed);
        try
        {
            await output.CopyToAsync(discord);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error while sending audio to ffmpeg with message: {e.Message}");
        }
        finally
        {
            await discord.FlushAsync();
        }
    }

    private static string ToParameters(Dictionary<string, string> parameters)
    {
        var result = new StringBuilder();
        foreach (var keyValuePair in parameters)
        {
            result.Append(keyValuePair.Key + "=" + keyValuePair.Value + "&");
        }

        return result.ToString().TrimEnd('&');
    }
}