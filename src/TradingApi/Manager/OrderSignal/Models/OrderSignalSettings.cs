namespace TradingApi.Manager.Storage.OrderSignal.Models;

public class OrderSignalSettings
{
    public required OrderSignalBuySettings BuySettings { get; init; } = new();
    public required OrderSignalSellSettings SellSettings { get; init; } = new ();
}
