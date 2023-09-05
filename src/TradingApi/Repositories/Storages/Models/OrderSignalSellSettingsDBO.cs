namespace TradingApi.Repositories.Storages.Models;

public record OrderSignalSellSettingsDBO
{
    public decimal? DifferencePositiveInPercent { get; init; }
    public decimal? DifferenceNegativeInPercent { get; init; }
    public TimeSpan? MaxDuration { get; init; }
}
