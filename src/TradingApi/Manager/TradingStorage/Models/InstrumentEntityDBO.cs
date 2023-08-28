using TradingApi.Repositories.MongoDb.Models;

namespace TradingApi.Manager.TradingStorage.Models;

public record InstrumentEntityDBO(
    string Isin,
    string? Name = null,
    string? Description = null
) : EntityDBO;
