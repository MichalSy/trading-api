using MediatR;
using System.Diagnostics.CodeAnalysis;
using TradingApi.Notifications;
using TradingApi.Repositories.ZeroRealtime;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.RealtimeQuotes;

public class RealtimeQuotesManager : IRealtimeQuotesManager, INotificationHandler<RealtimeQuoteReceived>
{
    private readonly IZeroRealtimeRepository _zeroRealtimeRepository;
    private readonly IPublisher _publisher;
    private Dictionary<string, List<RealtimeQuote>> _cacheQuotes = new();

    [SetsRequiredMembers]
    public RealtimeQuotesManager(IZeroRealtimeRepository zeroRealtimeRepository, IPublisher publisher)
    {
        _zeroRealtimeRepository = zeroRealtimeRepository;
        _publisher = publisher;
    }

    public Task Handle(RealtimeQuoteReceived notification, CancellationToken cancellationToken)
    {
        var quote = notification.Quote;
        if (!_cacheQuotes.TryGetValue(quote.Isin, out var currentQuotes))
        {
            var newList = new List<RealtimeQuote> { quote };
            _cacheQuotes.Add(quote.Isin, newList);
            _publisher.Publish(new RealtimeQuotesCacheUpdated(newList));
        }
        else
        {
            currentQuotes.RemoveAll(q => q.Timestamp < DateTime.UtcNow.AddHours(-2));
            currentQuotes.Add(quote);
            _publisher.Publish(new RealtimeQuotesCacheUpdated(currentQuotes));
        }

        return Task.CompletedTask;
    }

    public void StartAsync()
    {

    }
}
