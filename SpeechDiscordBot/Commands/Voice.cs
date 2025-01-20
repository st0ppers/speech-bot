using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using CSharpFunctionalExtensions;
using Discord;
using Discord.Audio;
using Discord.Commands;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using SpeechDiscordBot.Client;
using SpeechDiscordBot.Configuration;

namespace SpeechDiscordBot.Commands;

public class Voice(IOptions<ElevenLabsConfiguration> config, ElevenLabsClient client, ILogger logger) : ModuleBase<SocketCommandContext>
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

    //TODO Doesn't work in container. It cannot find opus dll 
    [Command("say", RunMode = RunMode.Async)]
    public async Task SayPhrase(params string[] text)
    {
        logger.Warning("Is the OS x64: {Answer}", Environment.Is64BitOperatingSystem);
        logger.Warning("OS for the current container is: {OperationSystem}", Environment.OSVersion);
        logger.Warning("Working directory is {Directory}", Directory.GetCurrentDirectory());
        var p = $"{Directory.GetCurrentDirectory()}\\{text[0].Replace(" ", string.Empty)}.mp3";
        if (File.Exists(p))
        {
            await ConnectAndPlay(p);
        }

        var content = JsonConvert.SerializeObject(new { text = text[0] });
        var c = new StringContent(content, Encoding.UTF8, config.Value.MediaType);
        var r = await client.PostAsync(string.Empty, c)
            .Tap(async x => await File.WriteAllBytesAsync(p, x))
            .TapError(async () => await ConnectAndPlay(p));

        // await Result.Try(() =>
        //             p.ToMaybe()
        //                 .Check(File.Exists, async path => await ConnectAndPlay(path))
        //                 .ToResult<string, Exception>(MaybeException.New())
        //                 .Map(_ =>
        //                 {
        //                     var content = JsonConvert.SerializeObject(new { text = text[0] });
        //                     return new StringContent(content, Encoding.UTF8, config.Value.MediaType);
        //                 })
        //                 .Bind(content => client.PostAsync(string.Empty, content))
        //                 .Tap(async x => await File.WriteAllBytesAsync(p, x))
        //                 .Tap(async () => await ConnectAndPlay(p))
        //                 .TapError(e => logger.Error("Error in say: {Message}", e.Message)),
        //         MaybeException.New)
        //     .TapError(e => logger.Error("Error in ResultTry: {Message}", e.Message));
    }

    private async Task ConnectAndPlay(string path)
    {
        try
        {
            var channel = (Context.User as IGuildUser)?.VoiceChannel;
            if (channel == null)
            {
                return;
            }

            var audioClient = await channel.ConnectAsync(true);
            await SendAsync(audioClient, path);
        }
        catch (Exception e)
        {
            logger.Error("Error in ConnectAndPlay: {Message}", e.Message);
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

    private async Task SendAsync(IAudioClient audioClient, string path)
    {
        using var ffmpeg = CreateStream(path);
        await using var output = ffmpeg?.StandardOutput.BaseStream;
        await using var discord = audioClient.CreatePCMStream(AudioApplication.Mixed);
        try
        {
            await output!.CopyToAsync(discord);
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