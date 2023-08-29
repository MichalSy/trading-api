using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.RealtimeQuotes;

public interface IRealtimeQuotesManager
{
    void RealtimeQuoteChangedReceived(RealtimeQuote quote);
    Task SubscribeIsinAsync(string isin);
}