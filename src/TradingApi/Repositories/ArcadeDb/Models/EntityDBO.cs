using System.Text.Json.Serialization;

namespace TradingApi.Repositories.ArcadeDb.Models;

public abstract record EntityDBO
{
    [JsonPropertyName("@rid")]
    public RecordId RecordId { get; init; }
}
