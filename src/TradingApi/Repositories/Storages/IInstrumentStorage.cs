using TradingApi.Repositories.Storages.Models;

namespace TradingApi.Repositories.Storages;

public interface IInstrumentStorage
{
    Task<InstrumentEntityDBO> CreateOrUpdateInstrumentAsync(InstrumentEntityDBO instrument);
    Task<InstrumentEntityDBO?> GetInstrumentAsync(string isin);
    Task StartAsync();
}