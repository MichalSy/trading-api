using MongoDB.Driver;
using TradingApi.Manager.Storage.InstrumentStorage.Models;
using TradingApi.Manager.Storage.TradingStorage.Models;
using TradingApi.Repositories.MongoDb;

namespace TradingApi.Manager.Storage.InstrumentStorage;

public class InstrumentStorageManager : IInstrumentStorageManager
{
    private readonly ILogger<InstrumentStorageManager> _logger;
    private readonly IMongoDbRepository _mongoDbRepository;

    [SetsRequiredMembers]
    public InstrumentStorageManager(ILogger<InstrumentStorageManager> logger, IMongoDbRepository mongoDbRepository)
    {
        _logger = logger;
        _mongoDbRepository = mongoDbRepository;
    }

    public async Task StartAsync()
    {
        await InitDatabaseStructureAsync();
        _logger.LogInformation("InstrumentStorageManager is ready!");
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

    public async Task<InstrumentEntityDBO?> CreateOrUpdateInstrumentAsync(InstrumentDTO instrument)
    {

        var collection = _mongoDbRepository.GetCollection<InstrumentEntityDBO>("Instruments");
        var entity = new InstrumentEntityDBO(instrument.Isin, "asd");
        await collection.InsertOneAsync(entity);
        return entity;
    }
}
