using System.Collections.Concurrent;
using TradingApi.Manager.OrderSignalDetector.Models;
using TradingApi.Manager.RealtimeQuotes;
using TradingApi.Manager.Storage.SignalDetector.Detectors;
using TradingApi.Manager.Storage.SignalDetector.Models;
using TradingApi.Repositories.Storages.SignalDetector;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.Storage.SignalDetector;

public class SignalDetectorManager : ISignalDetectorManager
{
    private ConcurrentBag<SignalDetectorJob> _loadedJobs = new();
    private readonly Dictionary<string, IOrderSignalDetector> _detectors;
    private readonly IServiceProvider _serviceProvider;
    private readonly ISignalDetectorStorage _signalDetectorStorage;

    [SetsRequiredMembers]
    public SignalDetectorManager(
        IEnumerable<IOrderSignalDetector> detectors, 
        IServiceProvider serviceProvider,
        ISignalDetectorStorage signalDetectorStorage)
    {
        _detectors = detectors.ToDictionary(d => d.Name);
        _serviceProvider = serviceProvider;
        _signalDetectorStorage = signalDetectorStorage;
    }

    public async Task StartAsync()
    {
        await LoadAllSignalDetectorJobsAsync();

    }

    private async Task LoadAllSignalDetectorJobsAsync()
    {
        var dbData = (await _signalDetectorStorage.GetSignalDetectorsAsync())?.Select(i => i.ToDTO());
        if (dbData is null)
            return;

        _loadedJobs = new ConcurrentBag<SignalDetectorJob>(dbData);

        await SubscribeAllInstrumentsAsync();
    }

    public async void AddSignalDetectorJobAsync(SignalDetectorJob job)
    {
        var newEntity = await _signalDetectorStorage.CreateOrUpdateSignalDetectorAsync(job.ToDBO());
        _loadedJobs.Add(newEntity.ToDTO());

        await SubscribeAllInstrumentsAsync();
    }

    private async Task SubscribeAllInstrumentsAsync()
    {
        var realtimeQuotesManager = _serviceProvider.GetRequiredService<IRealtimeQuotesManager>();
        foreach (var isin in _loadedJobs.Select(j => j.Isin).Distinct())
        {
            await realtimeQuotesManager.SubscribeIsinAsync(isin);
        }
    }

    public async Task ExecuteDetectorsAsync(RealtimeQuote LastQuote, IEnumerable<RealtimeQuote>? ChachedQuotes)
    {
        var affectedJobs = _loadedJobs.Where(j => j.Isin.Equals(LastQuote.Isin));
        await Parallel.ForEachAsync(
            affectedJobs,
            new ParallelOptions { MaxDegreeOfParallelism = 10 },
            async (job, token) =>
            {
                await _detectors[job.DetectorName].StartDetectAsync(job, LastQuote, ChachedQuotes);
            });
    }
}
