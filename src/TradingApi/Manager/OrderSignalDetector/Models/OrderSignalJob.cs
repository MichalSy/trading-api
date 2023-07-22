namespace TradingApi.Manager.OrderSignalDetector.Models;

public record OrderSignalJob(
    string Isin, 
    string DetectorName,
    Dictionary<string, object> Settings
);