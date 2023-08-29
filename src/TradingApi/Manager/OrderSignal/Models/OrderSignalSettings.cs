namespace TradingApi.Manager.Storage.OrderSignal.Models;

public record OrderSignalSettings
{
    public required OrderSignalBuySettings BuySettings { get; init; } = new();
    public required OrderSignalSellSettings SellSettings { get; init; } = new ();
}

public record OrderSignalBuySettings
{
    public int CoolDownAfterLastSellInSecs { get; init; } = 30;
    public int? StockCount { get; init; }
    public decimal? ValueInEur { get; init; }
    public bool RoundUpValueInEur { get; init; } = false;
}

public record OrderSignalSellSettings
{
    public decimal? DifferencePositiveInPercent { get; init; } = null;
    public decimal? DifferenceNegativeInPercent { get; init; } = null;
    public TimeSpan? MaxDuration { get; init; } = null;
}