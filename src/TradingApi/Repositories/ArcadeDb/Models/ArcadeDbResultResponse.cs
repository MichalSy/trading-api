namespace TradingApi.Repositories.ArcadeDb.Models;

public record ArcadeDbResultResponse<T>(IEnumerable<T> Result);