using MediatR;
using System.Diagnostics.CodeAnalysis;
using TradingApi.Notifications;
using TradingApi.Repositories.ZeroRealtime;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.RealtimeQuotesStorage;

public class RealtimeQuotesStorageManager : IRealtimeQuotesStorageManager, INotificationHandler<RealtimeQuoteReceived>
{
    private readonly IZeroRealtimeRepository _zeroRealtimeRepository;
    private readonly IPublisher _publisher;
    private Dictionary<string, List<RealtimeQuote>> _cacheQuotes = new();

    [SetsRequiredMembers]
    public RealtimeQuotesStorageManager(IZeroRealtimeRepository zeroRealtimeRepository, IPublisher publisher)
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
            _publisher.Publish(new RealtimeQuotesCacheUpdated(quote, null));
        }
        else
        {
            currentQuotes.RemoveAll(q => q.Timestamp < DateTime.UtcNow.AddHours(-2));
            _publisher.Publish(new RealtimeQuotesCacheUpdated(quote, currentQuotes.ToArray()));
            currentQuotes.Add(quote);
            
        }

        return Task.CompletedTask;
    }

    public void StartAsync()
    {

    }
}
