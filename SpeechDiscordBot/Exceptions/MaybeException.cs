namespace SpeechDiscordBot.Exceptions;

public sealed class MaybeException : Exception
{
    private MaybeException(string message) : base(message)
    {
    }

    public static MaybeException New()
    {
        return new MaybeException("Maybe has no value.");
    }

    public static MaybeException New(Exception e)
    {
        return new MaybeException(e.Message);
    }
}