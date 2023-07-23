using TradingApi.Manager.OrderSignalDetector.Models;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.OrderSignalDetector.Detectors;

public interface IOrderSignalDetector
{
    string DisplayName { get; }
    string Name { get; }

    Task DetectAsync(OrderSignalDetectorJob orderSignalJob, RealtimeQuote lastQuote, IEnumerable<RealtimeQuote>? cachedQuotes);
}
