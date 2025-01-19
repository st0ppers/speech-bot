namespace SpeechDiscordBot.Configuration;

public sealed class DiscordConfigruation
{
    public const string Section = "Discord";
    public string Token { get; set; }
    public string Prefix { get; set; }
    
}