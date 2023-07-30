using TradingApi.Repositories.ArcadeDb.Models;

namespace TradingApi.Manager.TradingStorage.Models;

public record InstrumentEntityDBO(
    string Isin,
    string Name,
    string Description
) : EntityDBO;
