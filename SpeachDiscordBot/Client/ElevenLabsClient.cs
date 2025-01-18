using Microsoft.Extensions.Options;
using SpeachDiscordBot.Configuration;

namespace SpeachDiscordBot.Client;

public sealed class ElevenLabsClient(IOptions<ElevenLabsConfiguration> options) : BaseClient(options.Value.BaseUrl + options.Value.OldManId + '?' + options.Value.Options.ToParameters(), "xi-api-key", options.Value.Key);