using TradingApi.Repositories.ArcadeDb.Models;

namespace TradingApi.Manager.TradingStorage.Models;

public record QuoteEntityDBO(
    DateTime Time,
    decimal Bid,
    decimal Ask
) : EntityDBO;
