using TradingApi.Repositories.MongoDb.Models;

namespace TradingApi.Repositories.Storages.SignalDetector.Models;

public record SignalDetectorEntityDBO : EntityDBO
{
    public required string Isin { get; init; }
    public required string DetectorName { get; init; }
    public required Dictionary<string, object> DetectorSettings { get; init; }
    public required OrderSignalSettingsDBO OrderSignalSettings { get; init; }
}

public record OrderSignalSettingsDBO
{
    public required OrderSignalBuySettingsDBO BuySettings { get; init; } = new();
    public required OrderSignalSellSettingsDBO SellSettings { get; init; } = new();
}

public record OrderSignalSellSettingsDBO
{
    public decimal? DifferencePositiveInPercent { get; init; }
    public decimal? DifferenceNegativeInPercent { get; init; }
    public TimeSpan? MaxDuration { get; init; }
}

public record OrderSignalBuySettingsDBO
{
    public int CoolDownAfterLastSellInSecs { get; init; } = 30;
    public int? StockCount { get; set; }
    public decimal? ValueInEur { get; set; }
    public bool RoundUpValueInEur { get; set; } = false;
}