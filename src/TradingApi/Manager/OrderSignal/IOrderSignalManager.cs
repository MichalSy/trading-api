using TradingApi.Manager.OrderSignal.Models;
using TradingApi.Manager.OrderSignalDetector.Models;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.OrderSignal;

public interface IOrderSignalManager
{
    Task<bool> CreateOrderSignalFromDetectorJobAsync(OrderSignalDetectorJob detectorJob, RealtimeQuote lastQuote);
    Task<OrderSignalJob?> GetLastOrderSignalsForDetectorJobIdAsync(Guid detectorJobid);
    Task StartAsync(CancellationToken stoppingToken);
    Task UpdateOrderSignalsAsync(RealtimeQuote LastQuote);
}