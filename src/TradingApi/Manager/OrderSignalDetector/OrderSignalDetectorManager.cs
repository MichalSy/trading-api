using System.Collections.Concurrent;
using TradingApi.Communication.Request;
using TradingApi.Manager.OrderSignal.Models;
using TradingApi.Manager.OrderSignalDetector.Detectors;
using TradingApi.Manager.OrderSignalDetector.Models;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.OrderSignalDetector;

public class OrderSignalDetectorManager : IOrderSignalDetectorManager
{
    private ConcurrentBag<OrderSignalDetectorJob> _loadedJobs = new();
    private readonly Dictionary<string, IOrderSignalDetector> _detectors;
    private readonly ISender _sender;

    public OrderSignalDetectorManager(ISender sender, IEnumerable<IOrderSignalDetector> detectors)
    {
        _sender = sender;
        _detectors = detectors.ToDictionary(d => d.Name);
    }

    public async Task StartAsync()
    {
        await LoadAllOrderSignalJobsAsync();

    }

    private Task LoadAllOrderSignalJobsAsync()
    {
        _loadedJobs = new(new OrderSignalDetectorJob[]
        {
            new OrderSignalDetectorJob(
                "US88160R1014", 
                "SimpleSlidingWindow", 
                new()
                {
                    { "WindowTimeInSecs", 40 },
                    { "BidDifferenceFromWindowStartInPercent", .05f },
                }, 
                new OrderSignalSettings
                {
                    BuySettings = new()
                    {
                        ValueInEur = 500,
                        RoundUpValueInEur = true,
                    },
                    SellSettings = new()
                    {
                        DifferencePositiveInPercent = 1m,
                        DifferenceNegativeInPercent = 0.4m
                    }
                }
            )
        });

        // register all instruments for realtime quotes
        foreach (var isin in _loadedJobs.Select(j => j.Isin).Distinct())
        {
            _sender.Send(new SubscribeIsinRequest(isin));
        }
        return Task.CompletedTask;
    }

    public async Task ExecuteDetectorsAsync(RealtimeQuote LastQuote, IEnumerable<RealtimeQuote>? ChachedQuotes)
    {
        var affectedJobs = _loadedJobs.Where(j => j.Isin.Equals(LastQuote.Isin));
        await Parallel.ForEachAsync(
            affectedJobs,
            new ParallelOptions { MaxDegreeOfParallelism = 10 },
            async (job, token) =>
            {
                await _detectors[job.DetectorName].DetectAsync(job, LastQuote, ChachedQuotes);
            });
    }
}
