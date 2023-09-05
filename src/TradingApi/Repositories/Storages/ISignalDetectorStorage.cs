using TradingApi.Repositories.Storages.Models;

namespace TradingApi.Repositories.Storages;

public interface ISignalDetectorStorage
{
    Task<SignalDetectorEntityDBO> CreateOrUpdateSignalDetectorAsync(SignalDetectorEntityDBO instrument);
    Task<SignalDetectorEntityDBO?> GetSignalDetectorAsync(Guid id);
    Task<IEnumerable<SignalDetectorEntityDBO>?> GetSignalDetectorsAsync();
    Task StartAsync();
}