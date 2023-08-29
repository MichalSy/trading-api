using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Repositories.ZeroRealtime;

public interface IZeroRealtimeRepository
{
    Task SubscribeIsinAsync(string isin);
    void SubscribeQuoteChange(Action<RealtimeQuote> action);
    Task UnsubscribeIsinAsync(string isin);
}