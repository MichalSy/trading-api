namespace TradingApi.Repositories.ZeroRealtime;

public interface IZeroRealtimeRepository
{
    void SubscribeIsinAsync(string isin);
    void UnsubscribeIsinAsync(string isin);
}