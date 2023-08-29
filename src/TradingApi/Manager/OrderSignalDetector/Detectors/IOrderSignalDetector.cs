using TradingApi.Manager.Storage.OrderSignalDetector.Models;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.Storage.OrderSignalDetector.Detectors;

public interface IOrderSignalDetector
{
    string DisplayName { get; }
    string Name { get; }

    Task StartDetectAsync(OrderSignalDetectorJob orderSignalJob, RealtimeQuote lastQuote, IEnumerable<RealtimeQuote>? cachedQuotes);
}
