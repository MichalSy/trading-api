using TradingApi.Repositories.ArcadeDb.Models;

namespace TradingApi.Repositories.ArcadeDb;

public static class ArcadeDbExtensoins
{
    public static IServiceCollection AddArcadeDb(this IServiceCollection services, Action<IServiceProvider, ArcadeDbRepositoryOptions> configure)
    {
        ArcadeDbRepositoryOptions options = new()
        {
            Host = string.Empty,
            Username = string.Empty,
            Password = string.Empty,
            Database = string.Empty,
        };

        services.Configure<ArcadeDbRepositoryOptions>(c => configure(services.BuildServiceProvider(), c));
        services.AddSingleton<IArcadeDbFactory, ArcadeDbFactory>();
        return services;
    }
}
