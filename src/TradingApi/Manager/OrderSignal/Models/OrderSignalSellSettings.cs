namespace TradingApi.Manager.OrderSignal.Models;

public class OrderSignalSellSettings
{
    public decimal? DifferenceInPercent { get; init; } = null;
    public TimeSpan? MaxDuration { get; init; } = null;
}
