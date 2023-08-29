using TradingApi.Repositories.Storages.Instrument.Models;

namespace TradingApi.Repositories.Storages.Instrument;

public interface IInstrumentStorage
{
    Task<InstrumentEntityDBO> CreateOrUpdateInstrumentAsync(InstrumentEntityDBO instrument);
    Task<InstrumentEntityDBO?> GetInstrumentAsync(string isin);
    Task StartAsync();
}