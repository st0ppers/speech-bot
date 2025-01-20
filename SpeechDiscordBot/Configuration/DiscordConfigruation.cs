namespace SpeechDiscordBot.Configuration;

public sealed class DiscordConfigruation
{
    public const string Section = "Discord";
    public required string Token { get; set; }
    public required string Prefix { get; set; }
    
}