using PushTechnology.ClientInterface.Client.Callbacks;
using PushTechnology.ClientInterface.Client.Factories;
using PushTechnology.ClientInterface.Client.Features;
using PushTechnology.ClientInterface.Client.Features.Topics;
using PushTechnology.ClientInterface.Client.Topics.Details;
using PushTechnology.ClientInterface.Data.JSON;
using System.Text.Json;
using System.Text.Json.Nodes;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Repositories.ZeroRealtime;

public class ZeroRealtimeRepository 
    : IZeroRealtimeRepository, IValueStream<IJSON>
{
    private readonly PushTechnology.ClientInterface.Client.Session.ISession _session;
    private readonly ILogger<ZeroRealtimeRepository> _logger;

    private readonly List<Action<RealtimeQuote>> _subscribers = new();

    [SetsRequiredMembers]
    public ZeroRealtimeRepository(ILogger<ZeroRealtimeRepository> logger, IConfiguration configuration)
    {
        _logger = logger;
        _session = Diffusion.Sessions
            .Principal(configuration["ZERODIF_USER"])
            .Password(configuration["ZERODIF_PASSWORD"])
            .Open(configuration["ZERODIF_SERVER"]);

        _session.Topics.AddFallbackStream(this);
        

        //_session.Topics.SubscribeAsync(">/tradeticker/lastx");
    }

    public Task SubscribeIsinAsync(string isin)
    {
        _session.Topics.SubscribeAsync($">wp/{isin}");
        return Task.CompletedTask;
    }

    public Task UnsubscribeIsinAsync(string isin)
    {
        _session.Topics.UnsubscribeAsync($">wp/{isin}");
        return Task.CompletedTask;
    }

    public void SubscribeQuoteChange(Action<RealtimeQuote> action)
        => _subscribers.Add(action);

    public void OnClose() { }

    public void OnError(ErrorReason errorReason)
        => _logger.LogError("ZeroRealtime - An error has occured: {errorReason}", errorReason);

    public void OnSubscription(string topicPath, ITopicSpecification specification)
        => _logger.LogInformation("ZeroRealtime - Client subscribed to {topicPath}", topicPath);

    public void OnUnsubscription(string topicPath, ITopicSpecification specification, TopicUnsubscribeReason reason)
        => _logger.LogInformation("ZeroRealtime - Client unsubscribed from {topicPath} : {reason}", topicPath, reason);

    public void OnValue(string topicPath, ITopicSpecification specification, IJSON oldValue, IJSON newValue)
    {
        string newDataJson = newValue.ToJSONString();
        _logger.LogTrace("ZeroRealtime - New ZeroRealtime Quote: {QuoteData}", newDataJson);

        var quote = JsonSerializer.Deserialize<Dictionary<string, JsonValue>>(newDataJson);
        if (quote is { })
        {
            var result = new RealtimeQuote
            (
              quote["i"].GetValue<string>(),
              DateTimeOffset.FromUnixTimeMilliseconds(quote["t"].GetValue<long>()).UtcDateTime,
              quote["b"].GetValue<decimal>(),
              quote["a"].GetValue<decimal>()
            );


            foreach (var subscriber in _subscribers)
                subscriber(result);
        }
    }
}
