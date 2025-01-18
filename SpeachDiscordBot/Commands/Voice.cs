using System.Diagnostics;
using System.Text;
using CSharpFunctionalExtensions;
using Discord;
using Discord.Audio;
using Discord.Commands;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SpeachDiscordBot.Client;
using SpeachDiscordBot.Configuration;
using SpeachDiscordBot.Exceptions;
using SpeachDiscordBot.Extensions;

namespace SpeachDiscordBot.Commands;

public class Voice(IOptions<ElevenLabsConfiguration> config, ElevenLabsClient client) : ModuleBase<SocketCommandContext>
{
    [Command("join", RunMode = RunMode.Async)]
    public async Task JoinChannel()
    {
        try
        {
            await ConnectAndPlay("RandomPath");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [Command("say", RunMode = RunMode.Async)]
    public async Task SayPhrase(params string[] text)
    {
        var p = $"{Directory.GetCurrentDirectory()}\\{text[0].Replace(" ", string.Empty)}.mp3";
        await Result.Try(() =>
                p.ToMaybe()
                    .Check(File.Exists, async path => await ConnectAndPlay(path))
                    .ToResult<string, Exception>(MaybeException.New())
                    .Map(_ =>
                    {
                        var content = JsonConvert.SerializeObject(new { text = text[0] });
                        return new StringContent(content, Encoding.UTF8, config.Value.MediaType);
                    })
                    .Bind(content => client.PostAsync(string.Empty, content))
                    .Tap(async x => await File.WriteAllBytesAsync(p, x))
                    .Tap(async () => await ConnectAndPlay(p)),
            MaybeException.New);
    }

    private async Task ConnectAndPlay(string path)
    {
        var channel = (Context.User as IGuildUser)?.VoiceChannel;
        if (channel == null)
        {
            return;
        }

        var audioClient = await channel.ConnectAsync(true);
        await SendAsync(audioClient, path);
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
}