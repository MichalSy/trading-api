namespace TradingApi.Repositories.ZeroRealtime.Models;

public record RealtimeQuote(
    string Isin, DateTime Timestamp, decimal Bid, decimal Ask);