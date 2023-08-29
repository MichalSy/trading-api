using TradingApi.Manager.Storage.TradingStorage.Models;

namespace TradingApi.Manager.Storage.InstrumentStorage;

public interface IInstrumentStorageManager
{
    Task<InstrumentEntityDBO> CreateOrUpdateInstrumentAsync(InstrumentEntityDBO instrument);
    Task<InstrumentEntityDBO?> GetInstrumentAsync(string isin);
    Task StartAsync();
}