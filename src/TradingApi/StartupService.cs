using TradingApi.Manager.OrderSignal;
using TradingApi.Manager.OrderSignalDetector;
using TradingApi.Manager.TradingStorage;

namespace TradingApi;

public class StartupService : BackgroundService
{
    private readonly ILogger<StartupService> _logger;
    private readonly ITradingStorageManager _tradingStorageManager;
    private readonly IOrderSignalManager _orderSignalManager;
    private readonly IOrderSignalDetectorManager _orderSignalDetectorManager;

    [SetsRequiredMembers]
    public StartupService(ILogger<StartupService> logger, ITradingStorageManager tradingStorageManager, IOrderSignalManager orderSignalManager, IOrderSignalDetectorManager orderSignalDetectorManager)
    {
        _logger = logger;
        _tradingStorageManager = tradingStorageManager;
        _orderSignalManager = orderSignalManager;
        _orderSignalDetectorManager = orderSignalDetectorManager;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _tradingStorageManager.StartAsync();
            await _orderSignalManager.StartAsync(stoppingToken);
            await _orderSignalDetectorManager.StartAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("Startup Error: {Exception}", ex);
        }
    }
}
