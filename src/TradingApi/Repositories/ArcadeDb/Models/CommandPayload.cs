using System.Text.Json.Serialization;

namespace TradingApi.Repositories.ArcadeDb.Models;

public class CommandPayload
{
    public required string Command { get; init; }

    [JsonPropertyName("params")]
    public object? Parameters { get; init; }

    public QueryLanguage Language { get; init; } = QueryLanguage.Sql;

    public string Serializer { get; init; } = "record";
}