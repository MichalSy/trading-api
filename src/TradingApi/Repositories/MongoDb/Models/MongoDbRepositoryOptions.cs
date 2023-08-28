namespace TradingApi.Repositories.MongoDb.Models;

public class MongoDbRepositoryOptions
{
    public required string Host { get; set; }
    public int Port { get; set; } = 27017;
    public required string Username { get; set; }
    public required string Password { get; set; }
    public string AuthDatabase { get; set; } = "admin";
    public required string Database { get; set; }
}