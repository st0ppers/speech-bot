using System.Text;

namespace SpeachDiscordBot.Configuration;

public static class Extensions
{

    public static string ToParameters(this Dictionary<string, string> parameters)
    {
        var result = new StringBuilder();
        foreach (var keyValuePair in parameters)
        {
            result.Append(keyValuePair.Key + "=" + keyValuePair.Value + "&");
        }

        return result.ToString().TrimEnd('&');
    }
}