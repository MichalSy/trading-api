using TradingApi.Repositories.MongoDb.Models;

namespace TradingApi.Repositories.Storages.Models;

public record InstrumentEntityDBO : EntityDBO
{
    public required string Isin { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
}
