namespace TradingApi.Repositories.ZeroRealtime.Models;

public record RealtimeQuote
{
    public required string Isin { get; init; }
    public required DateTime Timestamp { get; init; }
    public required decimal Bid { get; init; }
    public required decimal Ask { get; init; }
}