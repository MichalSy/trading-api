using MongoDB.Bson.Serialization.Attributes;

namespace TradingApi.Repositories.MongoDb.Models;

public abstract record EntityDBO
{
    [BsonId]
    public Guid Id { get; init; }
}
