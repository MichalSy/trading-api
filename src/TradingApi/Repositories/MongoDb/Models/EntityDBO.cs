using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TradingApi.Repositories.MongoDb.Models;

public abstract record EntityDBO
{
    [BsonId]
    public ObjectId Id { get; set; }
}
