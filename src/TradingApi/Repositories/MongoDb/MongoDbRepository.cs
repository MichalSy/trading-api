using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TradingApi.Repositories.MongoDb.Models;

namespace TradingApi.Repositories.MongoDb;

public class MongoDbRepository : IMongoDbRepository
{
    private readonly MongoClient _mongoClient;
    private readonly IMongoDatabase _mongoDatabase;
    private readonly ILogger<MongoDbRepository> _logger;

    [SetsRequiredMembers]
    public MongoDbRepository(IOptions<MongoDbRepositoryOptions> options, ILogger<MongoDbRepository> logger)
    {
        _logger = logger;

        var config = options.Value;
        _mongoClient = new MongoClient($"mongodb://{config.Username}:{config.Password}@{config.Host}:{config.Port}/{config.AuthDatabase}?directConnection=true");
        _mongoDatabase = _mongoClient.GetDatabase(config.Database);
    }

    public IMongoDatabase GetDatabase() 
        => _mongoDatabase;

    public IMongoCollection<T> GetCollection<T>(string collectionName) 
        => _mongoDatabase.GetCollection<T>(collectionName);

    public async void UseSessionAsync(Action<IClientSessionHandle> action)
    {
        using var session = await _mongoClient.StartSessionAsync();
        session.StartTransaction();

        try
        {
            await session.CommitTransactionAsync();
            action(session);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in MongoDb Session");
            await session.AbortTransactionAsync();
        }
    }
}
