namespace TradingApi.Repositories.Zero;

public interface IZeroRepository
{
    Task<object?> GetDepotPositionsAsync();
    Task<object?> GetWatchlistAsync();
    Task<string?> LoginAsync(string username, string password);
}