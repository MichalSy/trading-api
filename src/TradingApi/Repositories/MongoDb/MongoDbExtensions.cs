using MongoDB.Bson.Serialization.Conventions;
using TradingApi.Repositories.MongoDb.Models;

namespace TradingApi.Repositories.MongoDb;

public static class MongoDbExtensions
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services, Action<MongoDbRepositoryOptions> configure)
    {
        services.Configure(configure);
        services.AddSingleton<IMongoDbRepository, MongoDbRepository>();
        return services;
    }

    public static IServiceCollection AddMongoDb(this IServiceCollection services)
    {
        // don't save null values
        ConventionRegistry.Register("IgnoreIfDefault",
            new ConventionPack
            {
                new IgnoreIfDefaultConvention(true)
            },
            _ => true);

        var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
        services.AddMongoDb(c =>
        {
            c.Host = configuration["MONGODB_HOST"] ?? throw new Exception("MONGODB_HOST Env is missing");
            c.Username = configuration["MONGODB_USER"] ?? throw new Exception("MONGODB_USER Env is missing");
            c.Password = configuration["MONGODB_PASSWORD"] ?? throw new Exception("MONGODB_PASSWORD Env is missing");
            c.Database = configuration["MONGODB_DATABASE"] ?? throw new Exception("MONGODB_DATABASE Env is missing");

            if (configuration.GetValue<int?>("MONGODB_PORT") is not null)
            {
                c.Port = configuration.GetValue<int>("MONGODB_PORT")!;
            }

            if (!string.IsNullOrEmpty(configuration["MONGODB_AUTHDATABASE"]))
            {
                c.AuthDatabase = configuration["MONGODB_AUTHDATABASE"]!;
            }
        });
        return services;
    }
}
