namespace SpeechDiscordBot.Configuration;

public class ElevenLabsConfiguration
{
    public const string Section = "ElevenLabs";

    public required string BaseUrl { get; set; }
    public required Dictionary<string, string> Options { get; set; }
    public required string OldManId { get; set; }
    public required string Key { get; set; }
    public required string MediaType { get; set; }
}