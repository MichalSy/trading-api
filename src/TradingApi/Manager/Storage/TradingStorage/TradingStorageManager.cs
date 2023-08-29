using TradingApi.Manager.Storage.TradingStorage.Models;
using TradingApi.Repositories.MongoDb;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.Storage.TradingStorage;

public class TradingStorageManager : ITradingStorageManager
{
    private readonly ILogger<TradingStorageManager> _logger;
    private readonly IMongoDbRepository _mongoDbRepository;

    [SetsRequiredMembers]
    public TradingStorageManager(ILogger<TradingStorageManager> logger, IMongoDbRepository mongoDbRepository)
    {
        _logger = logger;
        _mongoDbRepository = mongoDbRepository;
    }

    public async Task StartAsync()
    {
        await InitDatabaseStructureAsync();
        _logger.LogInformation("TradingStorageManager is ready!");
    }

    private Task InitDatabaseStructureAsync()
    {
        return Task.CompletedTask;
    }

    public Task<QuoteEntityDBO?> SaveQuoteInDatabaseAsync(RealtimeQuote quote)
    {
        return Task.FromResult<QuoteEntityDBO?>(null);
    }
}
