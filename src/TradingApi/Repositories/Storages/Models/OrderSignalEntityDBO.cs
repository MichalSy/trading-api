using TradingApi.Repositories.MongoDb.Models;

namespace TradingApi.Repositories.Storages.Models;

public record OrderSignalEntityDBO : EntityDBO
{
    public DateTime CreatedDate { get; init; } = DateTime.UtcNow;
    public required Guid DetectorJobId { get; init; }
    public required string Isin { get; init; }
    public required int StockCount { get; init; }
    public required OrderSignalBuySettingsDBO BuySettings { get; init; }
    public required RealtimeQuoteDBO BuyQuote { get; init; }
    public required OrderSignalSellSettingsDBO SellSettings { get; init; }
    public RealtimeQuoteDBO? SellQuote { get; init; }

    public required decimal TotalBuyValueInEur { get; init; }
    public decimal? TotalSellValueInEur { get; init; }
    public decimal? TotalProfitInEur { get; init; }
}

