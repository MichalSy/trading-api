using TradingApi.Repositories.MongoDb.Models;

namespace TradingApi.Repositories.Storages.Models;

public record SignalDetectorEntityDBO : EntityDBO
{
    public required string Isin { get; init; }
    public required string DetectorName { get; init; }
    public required Dictionary<string, object> DetectorSettings { get; init; }
    public required OrderSignalSettingsDBO OrderSignalSettings { get; init; }
}
