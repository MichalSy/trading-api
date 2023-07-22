using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.RealtimeQuotesStorage;

public interface IRealtimeQuotesStorageManager
{
    Task CaptureQuoteAsync(RealtimeQuote quote);
}