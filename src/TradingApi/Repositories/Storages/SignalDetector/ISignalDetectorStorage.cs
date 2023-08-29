using TradingApi.Repositories.Storages.SignalDetector.Models;

namespace TradingApi.Repositories.Storages.SignalDetector;

public interface ISignalDetectorStorage
{
    Task<SignalDetectorEntityDBO> CreateOrUpdateSignalDetectorAsync(SignalDetectorEntityDBO instrument);
    Task<SignalDetectorEntityDBO?> GetSignalDetectorAsync(Guid id);
    Task StartAsync();
}