using TradingApi.Manager.OrderSignal.Models;
using TradingApi.Manager.OrderSignalDetector.Models;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.OrderSignal;

public interface IOrderSignalManager
{
    Task CreateOrderSignalFromDetectorJobAsync(OrderSignalDetectorJob detectorJob, RealtimeQuote lastQuote);
    Task<IEnumerable<OrderSignalJob>> GetActiveOrderSignalsForDetectorJobIdAsync(Guid detectorJobid);
    Task StartAsync(CancellationToken stoppingToken);
    Task UpdateOrderSignalsAsync(RealtimeQuote LastQuote);
}