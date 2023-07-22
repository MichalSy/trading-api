using MediatR;
using System.Collections.Concurrent;
using TradingApi.Communication.Commands;
using TradingApi.Manager.OrderSignalDetector.Detectors;
using TradingApi.Manager.OrderSignalDetector.Models;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.OrderSignalDetector;

public class OrderSignalDetectorManager : IOrderSignalDetectorManager
{
    private ConcurrentBag<OrderSignalJob> _loadedJobs = new();
    private readonly Dictionary<string, IOrderSignalDetector> _detectors;
    private readonly ISender _sender;

    public OrderSignalDetectorManager(ISender sender, IEnumerable<IOrderSignalDetector> detectors)
    {
        _detectors = detectors.ToDictionary(d => d.Name);
        _sender = sender;
    }

    public async Task StartAsync()
    {
        await LoadAllOrderSignalJobsAsync();

    }

    private Task LoadAllOrderSignalJobsAsync()
    {
        _loadedJobs = new(new OrderSignalJob[]
        {
            new OrderSignalJob("US88160R1014", "SimpleSlidingWindow", new()
            {
                { "WindowTimeInSecs", 20 },
                { "DifferenceToWindowStartInPercent", 1f },
                { "FinishOrderAfterDifferenceInPercent", 3f }
            })
        });

        // register all instruments for realtime quotes
        foreach (var isin in _loadedJobs.Select(j => j.Isin).Distinct())
        {
            _sender.Send(new SubscribeIsinCommand(isin));
        }
        return Task.CompletedTask;
    }

    public async Task ExecuteDetecotsAsync(RealtimeQuote LastQuote, IEnumerable<RealtimeQuote>? ChachedQuotes)
    {
        var affectedJobs = _loadedJobs.Where(j => j.Isin.Equals(LastQuote.Isin));
        await Parallel.ForEachAsync(
            affectedJobs,
            new ParallelOptions { MaxDegreeOfParallelism = 10 },
            async (job, token) =>
            {
                await _detectors[job.DetectorName].DetectAsync(job.Settings, LastQuote, ChachedQuotes);
            });
    }
}
