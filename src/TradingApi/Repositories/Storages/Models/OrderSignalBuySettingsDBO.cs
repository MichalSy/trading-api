namespace TradingApi.Repositories.Storages.Models;

public record OrderSignalBuySettingsDBO
{
    public int CoolDownAfterLastSellInSecs { get; init; } = 30;
    public int? StockCount { get; set; }
    public decimal? ValueInEur { get; set; }
    public bool RoundUpValueInEur { get; set; } = false;
}