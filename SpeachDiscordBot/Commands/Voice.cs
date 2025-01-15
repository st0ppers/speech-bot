using System.Diagnostics;
using Discord;
using Discord.Audio;
using Discord.Commands;

namespace SpeachDiscordBot.Commands;

public class Voice : ModuleBase<SocketCommandContext>
{
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