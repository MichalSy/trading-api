using Microsoft.AspNetCore.Mvc;
using TradingApi.Endpoints.ZeroRealtime.Models;
using TradingApi.Manager.RealtimeQuotes;
using TradingApi.Manager.Storage.SignalDetector;
using TradingApi.Manager.Storage.SignalDetector.Models;

namespace TradingApi.Endpoints.SignalDetector;

public static class SignalDetectorEndpoints
{
    public static void MapSignalDetectorEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("signaldetector")
            .WithTags("SignalDetector")
            .AllowAnonymous()
            .WithOpenApi();

        group.MapPost("/add", (ISignalDetectorManager signalDetectorManager, [FromBody] SignalDetectorJob job) =>
        {
            signalDetectorManager.AddSignalDetectorJobAsync(job);
        }).AllowAnonymous();
    }
}
