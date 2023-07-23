using TradingApi.Manager.OrderSignal;
using TradingApi.Manager.OrderSignalDetector;

namespace TradingApi;

public class ManagerBackgroundService : BackgroundService
{
    private readonly IOrderSignalManager _orderSignalManager;
    private readonly IOrderSignalDetectorManager _orderSignalDetectorManager;

    [SetsRequiredMembers]
    public ManagerBackgroundService(IOrderSignalManager orderSignalManager, IOrderSignalDetectorManager orderSignalDetectorManager)
    {
        _orderSignalManager = orderSignalManager;
        _orderSignalDetectorManager = orderSignalDetectorManager;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _orderSignalManager.StartAsync();
        await _orderSignalDetectorManager.StartAsync();
    }
}
