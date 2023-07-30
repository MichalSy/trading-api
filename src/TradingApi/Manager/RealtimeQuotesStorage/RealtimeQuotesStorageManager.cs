using TradingApi.Communication.Notification;
using TradingApi.Manager.TradingStorage;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.RealtimeQuotesStorage;

public class RealtimeQuotesStorageManager : IRealtimeQuotesStorageManager
{
    private readonly ILogger<RealtimeQuotesStorageManager> _logger;
    private readonly IPublisher _publisher;
    private readonly ITradingStorageManager _tradingStorageManager;
    private readonly Dictionary<string, List<RealtimeQuote>> _cacheQuotes = new();

    [SetsRequiredMembers]
    public RealtimeQuotesStorageManager(ILogger<RealtimeQuotesStorageManager> logger, IPublisher publisher, ITradingStorageManager tradingStorageManager)
    {
        _logger = logger;
        _publisher = publisher;
        _tradingStorageManager = tradingStorageManager;
    }

    public async Task CaptureQuoteAsync(RealtimeQuote quote)
    {
        // ignore quotes from different day
        if (quote.Timestamp.Date != DateTime.UtcNow.Date)
            return;

        _logger.LogInformation("Get Quote from isin: {isin}, ask: {ask} bid: {bid}", quote.Isin, quote.Ask, quote.Bid);

        if (!_cacheQuotes.TryGetValue(quote.Isin, out var currentQuotes))
        {
            var newList = new List<RealtimeQuote> { quote };
            _cacheQuotes.Add(quote.Isin, newList);
            await _publisher.Publish(new RealtimeQuotesCacheUpdatedNotification(quote, null));
        }
        else
        {
            currentQuotes.RemoveAll(q => q.Timestamp < DateTime.UtcNow.AddHours(-2));
            await _publisher.Publish(new RealtimeQuotesCacheUpdatedNotification(quote, currentQuotes.ToArray()));
            currentQuotes.Add(quote);
        }

        var a = await _tradingStorageManager.SaveQuoteInDatabaseAsync(quote);
    }

    // TODO: Need Quotes Cleanup after 2h + 10mins
}
