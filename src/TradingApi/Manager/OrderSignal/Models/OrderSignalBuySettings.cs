namespace TradingApi.Manager.Storage.OrderSignal.Models;

public class OrderSignalBuySettings
{
    public int CoolDownAfterLastSellInSecs { get; set; } = 30;
    public int? StockCount { get; set; }
    public decimal? ValueInEur { get; set; }
    public bool RoundUpValueInEur { get; set; } = false;
}
