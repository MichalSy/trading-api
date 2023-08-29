using MongoDB.Bson;
using MongoDB.Driver;
using TradingApi.Repositories.MongoDb;
using TradingApi.Repositories.Storages.SignalDetector.Models;

namespace TradingApi.Repositories.Storages.SignalDetector;

public class SignalDetectorStorage : ISignalDetectorStorage
{
    private readonly string _collectionName = "SignalDetectors";

    private readonly ILogger<SignalDetectorStorage> _logger;
    private readonly IMongoDbRepository _mongoDbRepository;
    private readonly IMongoCollection<SignalDetectorEntityDBO> _mongoCollection;

    [SetsRequiredMembers]
    public SignalDetectorStorage(ILogger<SignalDetectorStorage> logger, IMongoDbRepository mongoDbRepository)
    {
        _logger = logger;
        _mongoDbRepository = mongoDbRepository;
        _mongoCollection = _mongoDbRepository.GetCollection<SignalDetectorEntityDBO>(_collectionName);
    }

    public async Task StartAsync()
    {
        await InitDatabaseStructureAsync();
        _logger.LogInformation($"{nameof(SignalDetectorStorage)} is ready!");
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

        //// create unique index on Isin
        //await database.GetCollection<InstrumentEntityDBO>(_collectionName)
        //    .Indexes
        //    .CreateOneAsync(new CreateIndexModel<InstrumentEntityDBO>(
        //        Builders<InstrumentEntityDBO>.IndexKeys.Ascending(x => x.Isin),
        //            new CreateIndexOptions()
        //            {
        //                Unique = true
        //            }
        //        )
        //    );
    }

    public async Task<SignalDetectorEntityDBO> CreateOrUpdateSignalDetectorAsync(SignalDetectorEntityDBO instrument)
    {
        var loadedEntity = await GetSignalDetectorAsync(instrument.Id);
        var newEntity = instrument with { Id = loadedEntity?.Id ?? Guid.NewGuid() };

        await _mongoCollection.ReplaceOneAsync(i => i.Id == newEntity.Id, newEntity, new ReplaceOptions { IsUpsert = true });
        return newEntity;
    }

    public async Task<SignalDetectorEntityDBO?> GetSignalDetectorAsync(Guid id)
        => (await _mongoCollection.FindAsync(i => i.Id == id)).FirstOrDefault();
}
