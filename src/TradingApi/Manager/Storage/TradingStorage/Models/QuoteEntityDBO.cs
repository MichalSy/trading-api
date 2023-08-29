
using TradingApi.Repositories.MongoDb.Models;

namespace TradingApi.Manager.Storage.TradingStorage.Models;

public record QuoteEntityDBO(
    DateTime Time,
    decimal Bid,
    decimal Ask
) : EntityDBO;
