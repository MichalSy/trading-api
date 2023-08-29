using TradingApi.Manager.Storage.SignalDetector.Models;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.Storage.SignalDetector;

public interface ISignalDetectorManager
{
    void AddSignalDetectorJobAsync(SignalDetectorJob job);
    Task ExecuteDetectorsAsync(RealtimeQuote LastQuote, IEnumerable<RealtimeQuote>? ChachedQuotes);
    Task StartAsync();
}