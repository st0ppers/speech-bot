using System.Runtime.Serialization;
using System.Text.Json;
using CSharpFunctionalExtensions;
using Serilog;
using SpeechDiscordBot.Exceptions;

namespace SpeechDiscordBot.Client;

public abstract class BaseClient
{
    private readonly HttpClient _httpClient = new();
    private readonly ILogger _logger;

    protected BaseClient(string baseUrl, string tokenType, string token, ILogger logger)
    {
        _httpClient.BaseAddress = new Uri(baseUrl);
        _httpClient.DefaultRequestHeaders.Add(tokenType, token);
        _logger = logger;
    }

    public virtual async Task<Result<T, Exception>> GetAsync<T>(string endpoint)
    {
        var response = await _httpClient.GetAsync(endpoint);
        _logger.Information("Calling get service...");
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
                //Add metric
                _logger.Error("Failed to call service with error: {Message}", e.Message);
                return Result.Failure<T, Exception>(new SerializationException(e.Message));
            }
        }

        return HttpException.New(response.ReasonPhrase ?? string.Empty);
    }

    public virtual async Task<Result<byte[], Exception>> PostAsync(string endpoint, HttpContent content)
    {
        var response = await _httpClient.PostAsync(endpoint, content);
        _logger.Information("Calling post service...");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsByteArrayAsync();
        }

        _logger.Error("Failed to call service with error: {Phrase}", response.ReasonPhrase);
        return HttpException.New(response.ReasonPhrase ?? string.Empty);
    }
}