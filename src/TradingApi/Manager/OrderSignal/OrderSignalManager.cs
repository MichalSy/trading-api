using TradingApi.Manager.OrderSignalDetector.Models;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.OrderSignal;

public class OrderSignalManager : IOrderSignalManager
{
    public OrderSignalManager()
    {
        
    }

    public Task StartAsync()
    {
        return Task.CompletedTask;

    }

    public Task CreateOrderSignalFromDetectorJobAsync(OrderSignalDetectorJob orderSignalJob, RealtimeQuote lastQuote)
    {
        return Task.CompletedTask;
    }

    public Task UpdateOrderSignalsAsync(RealtimeQuote LastQuote)
    {
        return Task.CompletedTask;
    }
}
