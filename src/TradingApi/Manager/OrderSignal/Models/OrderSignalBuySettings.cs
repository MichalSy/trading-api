namespace TradingApi.Manager.OrderSignal.Models;

public class OrderSignalBuySettings
{
    public int? StockCount { get; set; }
    public decimal? ValueInEur { get; set; }
    public bool RoundUpValueInEur { get; set; } = false;
}
