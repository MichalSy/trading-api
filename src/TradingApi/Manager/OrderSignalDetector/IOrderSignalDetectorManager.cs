using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.OrderSignalDetector;

public interface IOrderSignalDetectorManager
{
    Task ExecuteDetectorsAsync(RealtimeQuote LastQuote, IEnumerable<RealtimeQuote>? ChachedQuotes);
    Task StartAsync();
}