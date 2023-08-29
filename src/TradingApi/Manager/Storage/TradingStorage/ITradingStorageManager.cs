using TradingApi.Manager.Storage.TradingStorage.Models;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.Storage.TradingStorage;

public interface ITradingStorageManager
{
    Task<QuoteEntityDBO?> SaveQuoteInDatabaseAsync(RealtimeQuote quote);
    Task StartAsync();
}