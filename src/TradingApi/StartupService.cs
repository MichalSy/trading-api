using TradingApi.Manager.Storage.OrderSignal;
using TradingApi.Manager.Storage.SignalDetector;
using TradingApi.Manager.Storage.TradingStorage;
using TradingApi.Repositories.Storages;

namespace TradingApi;

public class StartupService : BackgroundService
{
    private readonly ILogger<StartupService> _logger;
    private readonly ITradingStorageManager _tradingStorageManager;
    private readonly IInstrumentStorage _instrumentStorage;
    private readonly ISignalDetectorStorage _signalDetectorStorage;
    private readonly IOrderSignalManager _orderSignalManager;
    private readonly ISignalDetectorManager _orderSignalDetectorManager;

    [SetsRequiredMembers]
    public StartupService(
        ILogger<StartupService> logger, 
        ITradingStorageManager tradingStorageManager,
        IInstrumentStorage instrumentStorage,
        ISignalDetectorStorage signalDetectorStorage,
        IOrderSignalManager orderSignalManager, 
        ISignalDetectorManager orderSignalDetectorManager)
    {
        _logger = logger;
        _tradingStorageManager = tradingStorageManager;
        _instrumentStorage = instrumentStorage;
        _signalDetectorStorage = signalDetectorStorage;
        _orderSignalManager = orderSignalManager;
        _orderSignalDetectorManager = orderSignalDetectorManager;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _instrumentStorage.StartAsync();
            await _signalDetectorStorage.StartAsync();
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
