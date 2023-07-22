using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.OrderSignalDetector;

public interface IOrderSignalDetectorManager
{
    Task ExecuteDetecotsAsync(RealtimeQuote LastQuote, IEnumerable<RealtimeQuote>? ChachedQuotes);
    Task StartAsync();
}