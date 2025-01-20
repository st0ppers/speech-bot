using Microsoft.Extensions.Options;
using Serilog;
using SpeechDiscordBot.Configuration;

namespace SpeechDiscordBot.Client;

public sealed class ElevenLabsClient(IOptions<ElevenLabsConfiguration> options, ILogger logger)
    : BaseClient(options.Value.BaseUrl + options.Value.OldManId + '?' + options.Value.Options.ToParameters(), "xi-api-key", options.Value.Key, logger);