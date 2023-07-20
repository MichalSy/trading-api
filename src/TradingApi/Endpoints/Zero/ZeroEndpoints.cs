using Microsoft.AspNetCore.Mvc;
using TradingApi.Endpoints.Zero.Models;
using TradingApi.Repositories.Zero;

namespace TradingApi.Endpoints.Zero;

public static class ZeroEndpoints
{
    public static void MapZeroEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("zerotrade")
            .WithTags("ZeroTrade")
            .WithOpenApi();

        group.MapPost("/login", async (IZeroRepository repository, [FromBody] LoginRequest request) =>
        {
            return await repository.LoginAsync(request.Username, request.Password);
        }).AllowAnonymous();


        var watchlist = group.MapGroup("watchlist")
            .RequireAuthorization("ZeroTrade");

        watchlist.MapGet("list", async ([FromServices] IZeroRepository zeroRepository) =>
        {
            return await zeroRepository.GetWatchlistAsync();
        });


        var depot = group.MapGroup("depot")
            .RequireAuthorization("ZeroTrade");

        depot.MapGet("positions", async ([FromServices] IZeroRepository zeroRepository) =>
        {
            return await zeroRepository.GetDepotPositionsAsync();
        });
    }
}
