namespace TradingApi.Repositories.ArcadeDb.Models;

public class ArcadeDbRepositoryOptions
{
    public required string Host { get; set; }
    public int Port { get; set; } = 2480;
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Database { get; set; }
}
