namespace TradingApi.Manager.OrderSignalDetector.Models;

public record OrderSignalFinishCondition(
    decimal? DifferenceInPercent = null,
    TimeSpan? MaxDuration = null
);