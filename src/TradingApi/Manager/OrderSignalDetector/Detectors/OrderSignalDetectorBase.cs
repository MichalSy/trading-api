using System.Text.Json.Nodes;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.OrderSignalDetector.Detectors;

public abstract class OrderSignalDetectorBase
{
    protected virtual string Name => GetType().Name.Replace("Detector", string.Empty);
    protected virtual string DisplayName => Name;

    protected abstract Task DetectAsync(Dictionary<string, JsonValue> settings, RealtimeQuote lastQuote, IEnumerable<RealtimeQuote>? cachedQuotes);
}
