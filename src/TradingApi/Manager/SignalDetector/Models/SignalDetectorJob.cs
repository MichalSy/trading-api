using System.Text.Json.Serialization;
using TradingApi.Manager.Storage.OrderSignal.Models;

namespace TradingApi.Manager.Storage.SignalDetector.Models;

public record SignalDetectorJob
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public required string Isin { get; init; }
    public required string DetectorName { get; init; }

    public required Dictionary<string, object> DetectorSettings { get; init; }
    public required OrderSignalSettings OrderSignalSettings { get; init; }

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