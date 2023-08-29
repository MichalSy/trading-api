using System.Collections.Concurrent;
using TradingApi.Manager.RealtimeQuotes;
using TradingApi.Manager.Storage.OrderSignal.Models;
using TradingApi.Manager.Storage.OrderSignalDetector.Detectors;
using TradingApi.Manager.Storage.OrderSignalDetector.Models;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.Storage.OrderSignalDetector;

public class OrderSignalDetectorManager : IOrderSignalDetectorManager
{
    private ConcurrentBag<OrderSignalDetectorJob> _loadedJobs = new();
    private readonly Dictionary<string, IOrderSignalDetector> _detectors;
    private readonly IServiceProvider _serviceProvider;

    public OrderSignalDetectorManager(IEnumerable<IOrderSignalDetector> detectors, IServiceProvider serviceProvider)
    {
        _detectors = detectors.ToDictionary(d => d.Name);
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync()
    {
        await LoadAllOrderSignalDetectorJobsAsync();

    }

    private async Task LoadAllOrderSignalDetectorJobsAsync()
    {
        _loadedJobs = new(new OrderSignalDetectorJob[]
        {
            new OrderSignalDetectorJob(
                "US88160R1014",
                "SimpleSlidingWindow",
                new()
                {
                    { "WindowTimeInSecs", 60 },
                    { "BidDifferenceFromWindowStartInPercent", .15f },
                },
                new OrderSignalSettings
                {
                    BuySettings = new()
                    {
                        ValueInEur = 500,
                        RoundUpValueInEur = true,
                        CoolDownAfterLastSellInSecs = 40
                    },
                    SellSettings = new()
                    {
                        DifferencePositiveInPercent = 0.7m,
                        DifferenceNegativeInPercent = -0.4m
                    }
                }
            )
        });

        // register all instruments for realtime quotes
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
