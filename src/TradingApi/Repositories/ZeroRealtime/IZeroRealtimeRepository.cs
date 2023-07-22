namespace TradingApi.Repositories.ZeroRealtime;

public interface IZeroRealtimeRepository
{
    Task SubscribeIsinAsync(string isin);
    Task UnsubscribeIsinAsync(string isin);
}