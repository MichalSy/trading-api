using TradingApi.Manager.Storage.SignalDetector.Models;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.Storage.SignalDetector.Detectors;

public interface IOrderSignalDetector
{
    string DisplayName { get; }
    string Name { get; }

    Task StartDetectAsync(SignalDetectorJob orderSignalJob, RealtimeQuote lastQuote, IEnumerable<RealtimeQuote>? cachedQuotes);
}
