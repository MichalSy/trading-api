using MongoDB.Driver;
using TradingApi.Manager.TradingStorage.Models;
using TradingApi.Repositories.MongoDb;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.TradingStorage;

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

    private async Task InitDatabaseStructureAsync()
    {
        var database = _mongoDbRepository.GetDatabase();
        var collections = await (await database.ListCollectionNamesAsync()).ToListAsync();

        if (!collections.Contains("Instruments"))
        {
            await database.CreateCollectionAsync("Instruments");
        }

        await database.GetCollection<InstrumentEntityDBO>("Instruments")
            .Indexes
            .CreateOneAsync(new CreateIndexModel<InstrumentEntityDBO>(
                Builders<InstrumentEntityDBO>.IndexKeys.Ascending(x => x.Isin),
                new CreateIndexOptions()
                {
                    Unique = true
                }
                )
            );


    }

    public Task<QuoteEntityDBO?> SaveQuoteInDatabaseAsync(RealtimeQuote quote)
    {
        return Task.FromResult<QuoteEntityDBO?>(null);
    }

    public async Task<InstrumentEntityDBO?> CreateOrUpdateInstrumentInDatabaseAsync(InstrumentDTO instrument)
    {
        var collection = _mongoDbRepository.GetCollection<InstrumentEntityDBO>("Instruments");
        var entity = new InstrumentEntityDBO(instrument.Isin);
        await collection.InsertOneAsync(entity);

        return entity;
    }
}
