namespace TradingApi.Repositories.ArcadeDb;

public interface IArcadeDbFactory
{
    ArcadeDbConnection CreateConnection();
    ArcadeDbTransactionConnection CreateTransactionConnection();
}