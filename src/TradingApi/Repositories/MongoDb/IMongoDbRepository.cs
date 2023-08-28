using MongoDB.Driver;

namespace TradingApi.Repositories.MongoDb;

public interface IMongoDbRepository
{
    IMongoCollection<T> GetCollection<T>(string collectionName);
    IMongoDatabase GetDatabase();
    void UseSessionAsync(Action<IClientSessionHandle> action);
}