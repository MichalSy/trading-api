using MongoDB.Bson;
using MongoDB.Driver;
using TradingApi.Manager.Storage.TradingStorage.Models;
using TradingApi.Repositories.MongoDb;

namespace TradingApi.Manager.Storage.InstrumentStorage;

public class InstrumentStorageManager : IInstrumentStorageManager
{
    private readonly string _collectionName = "Instruments";

    private readonly ILogger<InstrumentStorageManager> _logger;
    private readonly IMongoDbRepository _mongoDbRepository;
    private readonly IMongoCollection<InstrumentEntityDBO> _mongoCollection;

    [SetsRequiredMembers]
    public InstrumentStorageManager(ILogger<InstrumentStorageManager> logger, IMongoDbRepository mongoDbRepository)
    {
        _logger = logger;
        _mongoDbRepository = mongoDbRepository;
        _mongoCollection = _mongoDbRepository.GetCollection<InstrumentEntityDBO>(_collectionName);
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

        // create collection if not exists
        if (!collections.Contains(_collectionName))
        {
            await database.CreateCollectionAsync(_collectionName);
        }

        // create unique index on Isin
        await database.GetCollection<InstrumentEntityDBO>(_collectionName)
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

    public async Task<InstrumentEntityDBO> CreateOrUpdateInstrumentAsync(InstrumentEntityDBO instrument)
    {
        var loadedEntity = await GetInstrumentAsync(instrument.Isin);
        var newEntity = new InstrumentEntityDBO(instrument.Isin)
        {
            Id = loadedEntity?.Id ?? ObjectId.GenerateNewId()
        };

        await _mongoCollection.ReplaceOneAsync(i => i.Isin.Equals(instrument.Isin), newEntity, new ReplaceOptions { IsUpsert = true });
        return newEntity;
    }

    public async Task<InstrumentEntityDBO?> GetInstrumentAsync(string isin) 
        => (await _mongoCollection.FindAsync(i => i.Isin.Equals(isin))).FirstOrDefault();
}
