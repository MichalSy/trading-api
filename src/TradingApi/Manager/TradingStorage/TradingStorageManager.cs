using Amazon.Runtime.Internal;
using System.Collections.Generic;
using TradingApi.Manager.TradingStorage.Models;
using TradingApi.Repositories.ArcadeDb;
using TradingApi.Repositories.ArcadeDb.Models;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.TradingStorage;

public class TradingStorageManager : ITradingStorageManager
{


    private readonly ILogger<TradingStorageManager> _logger;
    private readonly IArcadeDbFactory _arcadeDbFactory;

    [SetsRequiredMembers]
    public TradingStorageManager(ILogger<TradingStorageManager> logger, IArcadeDbFactory arcadeDbFactory)
    {
        _logger = logger;
        _arcadeDbFactory = arcadeDbFactory;
    }

    public async Task StartAsync()
    {
        await InitDatabaseStructureAsync();
        _logger.LogInformation("TradingStorageManager is ready!");
    }

    private async Task InitDatabaseStructureAsync()
    {
        var connection = _arcadeDbFactory.CreateConnection();

        // Init Instrument
        await connection.ExecuteSqlScriptAsync("""
            create vertex type Instrument if not exists;
            create property Instrument.Isin if not exists string (Mandatory true, NotNull true, Readonly true);
            create property Instrument.Name if not exists string (Mandatory true, NotNull true, Default NoName);
            create index if not exists on Instrument (Isin) unique;
            """);

        // Init Quote
        await connection.ExecuteSqlScriptAsync("""
            create vertex type Quote if not exists;
            create property Quote.Time if not exists DATETIME (Mandatory true, NotNull true, Readonly true);
            create property Quote.Ask if not exists DECIMAL (Mandatory true, NotNull true, Readonly true);
            create property Quote.Bid if not exists DECIMAL (Mandatory true, NotNull true, Readonly true);
            create index if not exists on Quote (Time) NotUnique;
            """);

        // Init Edge History
        await connection.ExecuteSqlScriptAsync("""
            CREATE EDGE TYPE QuoteHistory if not exists;
            """);
    }

    public async Task<QuoteEntityDBO?> SaveQuoteInDatabaseAsync(RealtimeQuote quote)
    {
        var connection = _arcadeDbFactory.CreateConnection();

        var response = await connection.ExecuteSqlScriptAsync("""
            let $quote = insert into Quote SET Time = :timestamp, Bid = :bid, Ask = :ask;
            let $instrument = SELECT FROM Instrument WHERE Isin = :isin;
            CREATE EDGE QuoteHistory FROM $instrument TO $quote;
            return $quote;
            """,
            quote
        );

        var dbResult = await response.Content.ReadFromJsonAsync<ArcadeDbResultResponse<QuoteEntityDBO>>();
        return dbResult?.Result.FirstOrDefault();
    }
}
