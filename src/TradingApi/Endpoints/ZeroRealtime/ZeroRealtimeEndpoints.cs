using Microsoft.AspNetCore.Mvc;
using TradingApi.Repositories.ZeroRealtime;

namespace TradingApi.Endpoints.ZeroRealtime;


public static class ZeroRealtimeEndpoints
{
    public static void MapZeroRealtimeEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("zrt")
            .WithTags("ZeroRealtime")
            .AllowAnonymous()
            .WithOpenApi();

        group.MapGet("/subscribe", (IZeroRealtimeRepository repository, [FromQuery] string isin) =>
        {
            repository.SubscribeIsinAsync(isin);
        });
    }
}
