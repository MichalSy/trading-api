using TradingApi.Manager.OrderSignalDetector.Models;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.OrderSignal;

public class OrderSignalManager : IOrderSignalManager
{

    public Task CreateOrderSignalFromDetectorJobAsync(OrderSignalDetectorJob orderSignalJob, RealtimeQuote lastQuote)
    {

        return Task.CompletedTask;
    }
}
