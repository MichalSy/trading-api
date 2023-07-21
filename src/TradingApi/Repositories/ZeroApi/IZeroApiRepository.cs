namespace TradingApi.Repositories.ZeroApi;

public interface IZeroApiRepository
{
    Task<object?> GetDepotPositionsAsync();
    Task<object?> GetWatchlistAsync();
    Task<string?> LoginAsync(string username, string password);
}