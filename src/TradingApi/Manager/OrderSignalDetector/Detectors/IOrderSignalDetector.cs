using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.OrderSignalDetector.Detectors;

public interface IOrderSignalDetector
{
    string DisplayName { get; }
    string Name { get; }

    Task DetectAsync(Dictionary<string, object> settings, RealtimeQuote lastQuote, IEnumerable<RealtimeQuote>? cachedQuotes);
}
