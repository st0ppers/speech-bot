namespace SpeachDiscordBot.Exceptions;

public sealed class HttpException : Exception
{
    private HttpException(string message) : base(message)
    {
    }

    public static HttpException New(string message) => new(message);
}