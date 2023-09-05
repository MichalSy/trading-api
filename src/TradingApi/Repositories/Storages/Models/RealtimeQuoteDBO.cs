namespace TradingApi.Repositories.Storages.Models;

public record RealtimeQuoteDBO
{
    public required string Isin { get; init; }
    public required DateTime Timestamp { get; init; }
    public required decimal Bid { get; init; }
    public required decimal Ask { get; init; }
}