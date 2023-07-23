namespace TradingApi.Manager.OrderSignalDetector.Models;

public record OrderSignalDetectorJob(
    string Isin,
    string DetectorName,
    Dictionary<string, object> DetectorSettings,
    OrderSignalFinishCondition FinishCondition
)
{
    public T GetDetectorSettingValue<T>(string key, T defaultValue = default)
        where T : struct
    {
        if (DetectorSettings.TryGetValue(key, out var value))
        {
            return (T)value;
        }
        return defaultValue;
    }
}