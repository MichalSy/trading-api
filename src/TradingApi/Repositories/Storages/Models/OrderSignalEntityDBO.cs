using TradingApi.Repositories.MongoDb.Models;

namespace TradingApi.Repositories.Storages.Models;

public record OrderSignalEntityDBO : EntityDBO
{
    public required DateTime CreatedDate { get; init; } = DateTime.UtcNow;
    public required Guid DetectorJobId { get; init; }
    public required string Isin { get; init; }
    public required int StockCount { get; init; }
    public required OrderSignalBuySettingsDBO BuySettings { get; init; }
    public required RealtimeQuoteDBO BuyQuote { get; init; }
    public required OrderSignalSellSettingsDBO SellSettings { get; init; }
    public required RealtimeQuoteDBO? SellQuote { get; init; }
}

