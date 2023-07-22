using MediatR;
using System.Collections.Concurrent;
using System.Text.Json.Nodes;
using TradingApi.Manager.OrderSignalDetector.Detectors;
using TradingApi.Manager.OrderSignalDetector.Models;
using TradingApi.Notifications;

namespace TradingApi.Manager.OrderSignalDetector;

public class OrderSignalDetectorManager : IOrderSignalDetectorManager, INotificationHandler<RealtimeQuotesCacheUpdated>
{
    private ConcurrentBag<OrderSignalJob> _loadedJobs = new();
    private readonly Dictionary<string, IOrderSignalDetector> _detectors;

    public OrderSignalDetectorManager(IEnumerable<IOrderSignalDetector> detectors)
    {
        LoadAllOrderSignalJobsAsync();
        _detectors = detectors.ToDictionary(d => d.Name);
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
        return Task.CompletedTask;
    }

    public Task Handle(RealtimeQuotesCacheUpdated notification, CancellationToken cancellationToken)
    {
        var affectedJobs = _loadedJobs.Where(j => j.Isin.Equals(notification.LastQuote.Isin));
        foreach (var job in affectedJobs.ToList())
        {
            _detectors[job.DetectorName].DetectAsync(job.Settings, notification.LastQuote, notification.ChachedQuotes);
        }
        return Task.CompletedTask;
    }
}
