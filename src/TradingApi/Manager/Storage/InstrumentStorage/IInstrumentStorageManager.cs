using TradingApi.Manager.Storage.InstrumentStorage.Models;
using TradingApi.Manager.Storage.TradingStorage.Models;

namespace TradingApi.Manager.Storage.InstrumentStorage;

public interface IInstrumentStorageManager
{
    Task<InstrumentEntityDBO?> CreateOrUpdateInstrumentAsync(InstrumentDTO instrument);
    Task StartAsync();
}