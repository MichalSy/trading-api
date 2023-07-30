using Microsoft.AspNetCore.Mvc;
using TradingApi.Communication.Notification;
using TradingApi.Communication.Request;
using TradingApi.Endpoints.ZeroRealtime.Models;
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

        group.MapGet("/subscribe", (ISender sender, [FromQuery] string isin) =>
        {
            sender.Send(new SubscribeIsinRequest(isin));
        });

        group.MapPost("/simulate-quote", (IPublisher publisher, [FromBody] SimulateQuoteRequest request) =>
        {
            publisher.Publish(new RealtimeQuoteReceivedNotification(new RealtimeQuote(
                request.Isin,
                request.Timestamp,
                request.Bid,
                request.Ask
            )));
        }).AllowAnonymous();
    }
}
