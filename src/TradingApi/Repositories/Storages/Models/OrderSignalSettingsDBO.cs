namespace TradingApi.Repositories.Storages.Models;

public record OrderSignalSettingsDBO
{
    public required OrderSignalBuySettingsDBO BuySettings { get; init; } = new();
    public required OrderSignalSellSettingsDBO SellSettings { get; init; } = new();
}
