using MongoDB.Bson;
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

        var loadedEntity = (await collection.FindAsync(i => i.Isin.Equals(instrument.Isin))).FirstOrDefault();
        var newEntity = new InstrumentEntityDBO(instrument.Isin)
        {
            Id = loadedEntity?.Id ?? ObjectId.GenerateNewId()
        };

        await collection.ReplaceOneAsync(i => i.Isin.Equals(instrument.Isin), newEntity, new ReplaceOptions { IsUpsert = true });
        return newEntity;
    }
}
