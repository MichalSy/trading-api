using TradingApi.Manager.TradingStorage.Models;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.TradingStorage;

public interface ITradingStorageManager
{
    Task<QuoteEntityDBO?> SaveQuoteInDatabaseAsync(RealtimeQuote quote);
    Task StartAsync();
}