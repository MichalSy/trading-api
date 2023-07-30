namespace TradingApi.Endpoints.ZeroRealtime.Models;

public class SimulateQuoteRequest
{
    public required string Isin { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public decimal Bid { get; init; }
    public decimal Ask { get; init; }
}
