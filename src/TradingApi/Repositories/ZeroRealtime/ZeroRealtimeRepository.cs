using MediatR;
using PushTechnology.ClientInterface.Client.Callbacks;
using PushTechnology.ClientInterface.Client.Factories;
using PushTechnology.ClientInterface.Client.Features;
using PushTechnology.ClientInterface.Client.Features.Topics;
using PushTechnology.ClientInterface.Client.Topics.Details;
using PushTechnology.ClientInterface.Data.JSON;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;
using TradingApi.Notifications;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Repositories.ZeroRealtime;

public class ZeroRealtimeRepository : IZeroRealtimeRepository, IValueStream<IJSON>
{
    private readonly PushTechnology.ClientInterface.Client.Session.ISession _session;
    private readonly ILogger<ZeroRealtimeRepository> _logger;
    private readonly IPublisher _publisher;

    [SetsRequiredMembers]
    public ZeroRealtimeRepository(ILogger<ZeroRealtimeRepository> logger, IConfiguration configuration, IPublisher publisher)
    {
        _logger = logger;
        _publisher = publisher;
        _session = Diffusion.Sessions
            .Principal(configuration["ZERODIF_USER"])
            .Password(configuration["ZERODIF_PASSWORD"])
            .Open(configuration["ZERODIF_SERVER"]);

        _session.Topics.AddFallbackStream(this);
        

        //_session.Topics.SubscribeAsync(">/tradeticker/lastx");
    }

    public void SubscribeIsinAsync(string isin)
    {
        _session.Topics.SubscribeAsync($">wp/{isin}");
    }

    public void UnsubscribeIsinAsync(string isin)
    {
        _session.Topics.UnsubscribeAsync($">wp/{isin}");
    }

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

            _publisher.Publish(new RealtimeQuoteReceived(result));
        }
    }
}
