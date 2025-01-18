namespace SpeachDiscordBot.Configuration;

public class ElevenLabsConfiguration
{
    public const string Section = "ElevenLabs";

    public string BaseUrl { get; set; }
    public Dictionary<string, string> Options { get; set; }
    public string OldManId { get; set; }
    public string Key { get; set; }
    public string MediaType { get; set; }
}