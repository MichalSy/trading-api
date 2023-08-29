using Microsoft.AspNetCore.Mvc;
using TradingApi.Endpoints.ZeroRealtime.Models;
using TradingApi.Manager.RealtimeQuotes;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Endpoints.ZeroRealtime;


public static class ZeroRealtimeEndpoints
{
    public static void MapZeroRealtimeEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("zrt")
            .WithTags("ZeroRealtime")
            .AllowAnonymous()
            .WithOpenApi();

        group.MapGet("/subscribe", (IRealtimeQuotesManager realtimeQuotesManager, [FromQuery] string isin) =>
        {
            realtimeQuotesManager.SubscribeIsinAsync(isin);
        });

        group.MapPost("/simulate-quote", (IRealtimeQuotesManager realtimeQuotesManager, [FromBody] SimulateQuoteRequest request) =>
        {
            realtimeQuotesManager.RealtimeQuoteChangedReceived(new RealtimeQuote(
                request.Isin,
                request.Timestamp,
                request.Bid,
                request.Ask
            ));
        }).AllowAnonymous();
    }
}
