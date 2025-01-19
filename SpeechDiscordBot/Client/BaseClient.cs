using System.Runtime.Serialization;
using System.Text.Json;
using CSharpFunctionalExtensions;
using SpeechDiscordBot.Exceptions;

namespace SpeechDiscordBot.Client;

public abstract class BaseClient
{
    private readonly HttpClient _httpClient = new();

    protected BaseClient(string baseUrl, string tokenType, string token)
    {
        _httpClient.BaseAddress = new Uri(baseUrl);
        _httpClient.DefaultRequestHeaders.Add(tokenType, token);
    }

    public virtual async Task<Result<T, Exception>> GetAsync<T>(string endpoint)
    {
        var response = await _httpClient.GetAsync(endpoint);
        if (response.IsSuccessStatusCode)
        {
            var r = await response.Content.ReadAsStringAsync();
            try
            {
                //!! Possible Null
                return JsonSerializer.Deserialize<T>(r)!;
            }
            catch (Exception e)
            {
                //TODO
                //Add log
                //Add metric
                return Result.Failure<T, Exception>(new SerializationException(e.Message));
            }
        }

        return HttpException.New(response.ReasonPhrase ?? string.Empty);
    }

    public virtual async Task<Result<byte[], Exception>> PostAsync(string endpoint, HttpContent content)
    {
        var response = await _httpClient.PostAsync(endpoint, content);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsByteArrayAsync();
        }

        return HttpException.New(response.ReasonPhrase ?? string.Empty);
    }
}