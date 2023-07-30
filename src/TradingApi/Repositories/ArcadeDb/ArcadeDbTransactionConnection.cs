using System.Text.Json;

namespace TradingApi.Repositories.ArcadeDb;

public class ArcadeDbTransactionConnection : ArcadeDbConnection
{
    [SetsRequiredMembers]
    public ArcadeDbTransactionConnection(HttpClient httpClient, string database, JsonSerializerOptions jsonSerializerOptions) 
        : base(httpClient, database, jsonSerializerOptions)
    {
    }

    // TODO: Implement :D
}
