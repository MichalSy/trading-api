using MongoDB.Driver;
using TradingApi.Repositories.MongoDb;
using TradingApi.Repositories.Storages.Models;

namespace TradingApi.Repositories.Storages;

public class OrderSignalStorage : IOrderSignalStorage
{
    private readonly string _collectionName = "OrderSignals";

    private readonly ILogger<SignalDetectorStorage> _logger;
    private readonly IMongoDbRepository _mongoDbRepository;
    private readonly IMongoCollection<OrderSignalEntityDBO> _mongoCollection;

    [SetsRequiredMembers]
    public OrderSignalStorage(ILogger<SignalDetectorStorage> logger, IMongoDbRepository mongoDbRepository)
    {
        _logger = logger;
        _mongoDbRepository = mongoDbRepository;
        _mongoCollection = _mongoDbRepository.GetCollection<OrderSignalEntityDBO>(_collectionName);
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

        // create indexes
        var indexes = database.GetCollection<OrderSignalEntityDBO>(_collectionName).Indexes;
        await indexes.CreateManyAsync(new[] {
                new CreateIndexModel<OrderSignalEntityDBO>(Builders<OrderSignalEntityDBO>.IndexKeys.Ascending(x => x.Isin), null),
                new CreateIndexModel<OrderSignalEntityDBO>(Builders<OrderSignalEntityDBO>.IndexKeys.Ascending(x => x.DetectorJobId), null)
            }
        );
    }

    public async Task<OrderSignalEntityDBO> CreateOrUpdateOrderSignalAsync(OrderSignalEntityDBO orderSignalDBO)
    {
        _logger.LogInformation($"CreateOrUpdateOrderSignalAsync: {orderSignalDBO.Id}");
        var loadedEntity = await GetOrderSignalAsync(orderSignalDBO.Id);
        _logger.LogInformation($"Loaded Entity: {loadedEntity?.Id}");
        var newEntity = orderSignalDBO with { Id = loadedEntity?.Id ?? orderSignalDBO.Id };

        await _mongoCollection.ReplaceOneAsync(i => i.Id == newEntity.Id, newEntity, new ReplaceOptions { IsUpsert = true });
        return newEntity;
    }

    public async Task<IEnumerable<OrderSignalEntityDBO>?> GetOrderSignalsAsync()
        => (await _mongoCollection.FindAsync(_ => true)).ToEnumerable();

    public async Task<OrderSignalEntityDBO?> GetOrderSignalAsync(Guid id)
        => (await _mongoCollection.FindAsync(i => i.Id == id)).FirstOrDefault();
}