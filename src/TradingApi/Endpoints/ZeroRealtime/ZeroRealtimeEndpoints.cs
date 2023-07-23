using Microsoft.AspNetCore.Mvc;
using TradingApi.Communication.Commands;
using TradingApi.Communication.Notification;
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
            sender.Send(new SubscribeIsinCommand(isin));
        });

        group.MapPost("/simulate-quote", (IPublisher publisher, [FromBody] RealtimeQuote request) =>
        {
            publisher.Publish(new RealtimeQuoteReceivedNotification(request));
        }).AllowAnonymous();
    }
}
