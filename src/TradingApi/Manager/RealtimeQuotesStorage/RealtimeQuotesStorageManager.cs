using MediatR;
using System.Diagnostics.CodeAnalysis;
using TradingApi.Communication.NotificationHandler;
using TradingApi.Repositories.ZeroRealtime;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.RealtimeQuotesStorage;

public class RealtimeQuotesStorageManager : IRealtimeQuotesStorageManager
{
    private readonly IPublisher _publisher;
    private readonly Dictionary<string, List<RealtimeQuote>> _cacheQuotes = new();

    [SetsRequiredMembers]
    public RealtimeQuotesStorageManager(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public Task CaptureQuoteAsync(RealtimeQuote quote)
    {
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
}
