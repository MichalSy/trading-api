namespace TradingApi.Manager.Storage.OrderSignal.Models;

public class OrderSignalSellSettings
{
    public decimal? DifferencePositiveInPercent { get; init; } = null;
    public decimal? DifferenceNegativeInPercent { get; init; } = null;
    public TimeSpan? MaxDuration { get; init; } = null;
}
