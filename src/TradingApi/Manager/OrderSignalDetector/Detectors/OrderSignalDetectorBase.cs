using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.OrderSignalDetector.Detectors;

public abstract class OrderSignalDetectorBase : IOrderSignalDetector
{
    public virtual string Name => GetType().Name.Replace("Detector", string.Empty);

    public virtual string DisplayName => Name;

    public abstract Task DetectAsync(Dictionary<string, object> settings, RealtimeQuote lastQuote, IEnumerable<RealtimeQuote>? cachedQuotes);
}
