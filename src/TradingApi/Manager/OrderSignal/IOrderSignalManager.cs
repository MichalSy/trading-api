using TradingApi.Manager.OrderSignalDetector.Models;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.OrderSignal;

public interface IOrderSignalManager
{
    Task CreateOrderSignalFromDetectorJobAsync(OrderSignalDetectorJob orderSignalJob, RealtimeQuote lastQuote);
}