using Amazon.CognitoIdentityProvider.Model.Internal.MarshallTransformations;
using System.Collections.Concurrent;
using TradingApi.Manager.OrderSignal.Models;
using TradingApi.Manager.OrderSignalDetector.Models;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.OrderSignal;

public class OrderSignalManager : IOrderSignalManager
{
    private ConcurrentBag<OrderSignalJob> _orderSignals = new();

    public OrderSignalManager()
    {
        
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _ = CleanupClosedJobsIntervalTask(stoppingToken);

        return Task.CompletedTask;
    }

    public Task CreateOrderSignalFromDetectorJobAsync(OrderSignalDetectorJob detectorJob, RealtimeQuote lastQuote)
    {
        _orderSignals.Add(new OrderSignalJob
        {
            DetectorJob = detectorJob,
            BuyQuote = lastQuote
        });

        return Task.CompletedTask;
    }

    public async Task UpdateOrderSignalsAsync(RealtimeQuote lastQuote)
    {
        var affectedJobs = _orderSignals.Where(o => o.Isin.Equals(lastQuote.Isin) && !o.IsClosed);
        await Parallel.ForEachAsync(
            affectedJobs,
            new ParallelOptions { MaxDegreeOfParallelism = 10 },
            async (job, token) =>
            {
                await UpdateJobAsync(job, lastQuote);
            });
    }

    private async Task CleanupClosedJobsIntervalTask(CancellationToken stoppingToken)
    {
        while(!stoppingToken.IsCancellationRequested)
        {
            // cleanup all closed jobs
            _orderSignals = new ConcurrentBag<OrderSignalJob>(_orderSignals.Where(o => !o.IsClosed));
            await Task.Delay(60000, stoppingToken); // 1min
        }
    }

    private Task UpdateJobAsync(OrderSignalJob signalJob, RealtimeQuote lastQuote)
    {
        return Task.CompletedTask;
    }
}
