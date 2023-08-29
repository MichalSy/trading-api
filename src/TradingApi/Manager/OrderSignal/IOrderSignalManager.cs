using TradingApi.Manager.Storage.OrderSignal.Models;
using TradingApi.Manager.Storage.SignalDetector.Models;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.Storage.OrderSignal;

public interface IOrderSignalManager
{
    Task<bool> CreateOrderSignalFromDetectorJobAsync(SignalDetectorJob detectorJob, RealtimeQuote lastQuote);
    Task<OrderSignalJob?> GetLastOrderSignalsForDetectorJobIdAsync(Guid detectorJobid);
    Task StartAsync(CancellationToken stoppingToken);
    Task UpdateOrderSignalsAsync(RealtimeQuote LastQuote);
}