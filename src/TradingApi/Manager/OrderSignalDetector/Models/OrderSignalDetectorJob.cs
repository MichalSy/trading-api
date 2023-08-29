using TradingApi.Manager.Storage.OrderSignal.Models;

namespace TradingApi.Manager.Storage.OrderSignalDetector.Models;

public record OrderSignalDetectorJob(
    string Isin,
    string DetectorName,
    Dictionary<string, object> DetectorSettings,
    OrderSignalSettings OrderSignalSettings
)
{
    public Guid Id { get; } = Guid.NewGuid();

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