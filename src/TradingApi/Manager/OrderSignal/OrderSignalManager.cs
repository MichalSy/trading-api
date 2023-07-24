using Amazon.CognitoIdentityProvider.Model.Internal.MarshallTransformations;
using System.Collections.Concurrent;
using TradingApi.Manager.OrderSignal.Models;
using TradingApi.Manager.OrderSignalDetector.Models;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.OrderSignal;

public class OrderSignalManager : IOrderSignalManager
{
    private ConcurrentBag<OrderSignalJob> _orderSignals = new();
    private readonly ILogger<OrderSignalManager> _logger;

    [SetsRequiredMembers]
    public OrderSignalManager(ILogger<OrderSignalManager> logger)
    {
        _logger = logger;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _ = CleanupClosedJobsIntervalTask(stoppingToken);

        return Task.CompletedTask;
    }

    public async Task CreateOrderSignalFromDetectorJobAsync(OrderSignalDetectorJob detectorJob, RealtimeQuote lastQuote)
    {
        await BuyStockAsync(detectorJob, lastQuote);
    }

    public Task<IEnumerable<OrderSignalJob>> GetActiveOrderSignalsForDetectorJobIdAsync(Guid detectorJobid)
    {
        return Task.FromResult(_orderSignals.Where(o => o.DetectorJob.Id.Equals(detectorJobid)));
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

    private async Task UpdateJobAsync(OrderSignalJob signalJob, RealtimeQuote lastQuote)
    {
        if (signalJob.BuyQuote is null)
            return;

        // check if positive difference are greater 
        var differenceBuyPriceAndLastBidPrice = lastQuote.Bid / signalJob.BuyQuote.Ask * 100 - 100;
        _logger.LogTrace("Difference between BuyAskPrice ({BuyAskPrice}) and LastBidPrice ({LastBidPrice}), Diff: {differenceBuyPriceAndLastBidPrices:N3}",
                               signalJob.BuyQuote.Ask, 
                               lastQuote.Bid, 
                               differenceBuyPriceAndLastBidPrice);

        if (differenceBuyPriceAndLastBidPrice > signalJob.DetectorJob.OrderSignalSettings.SellSettings.DifferencePositiveInPercent)
        {
            await SellStockAsync(signalJob, lastQuote);
            return;
        }

        if (differenceBuyPriceAndLastBidPrice < signalJob.DetectorJob.OrderSignalSettings.SellSettings.DifferenceNegativeInPercent)
        {
            await SellStockAsync(signalJob, lastQuote);
            return;
        }
    }

    private Task BuyStockAsync(OrderSignalDetectorJob detectorJob, RealtimeQuote lastQuote)
    {
        var stockAmount = 0;
        if (detectorJob.OrderSignalSettings.BuySettings.ValueInEur >= 0)
        {
            if (detectorJob.OrderSignalSettings.BuySettings.RoundUpValueInEur)
            {
                stockAmount = (int)Math.Ceiling(detectorJob.OrderSignalSettings.BuySettings.ValueInEur.Value / lastQuote.Ask);
            }
            else
            {
                stockAmount = (int)Math.Floor(detectorJob.OrderSignalSettings.BuySettings.ValueInEur.Value / lastQuote.Ask);
            }
        }

        if (stockAmount <= 0)
            return Task.CompletedTask;

        var newJob = new OrderSignalJob
        {
            DetectorJob = detectorJob
        };
        newJob.BuyStockCount(lastQuote, stockAmount);

        _orderSignals.Add(newJob);

        _logger.LogInformation("Buy {StockCount} x {BuyPrice:N2}€ = {TotalValue:N2}€", stockAmount, lastQuote.Ask, stockAmount * lastQuote.Ask);
        return Task.CompletedTask;
    }

    private Task SellStockAsync(OrderSignalJob signalJob, RealtimeQuote lastQuote)
    {
        if (signalJob.BuyQuote is null || signalJob.BuyedStockCount is null)
            return Task.CompletedTask;

        //_logger.LogInformation("Verkaufen {a}, kaufen {b}", lastQuote.Bid * signalJob.BuyedStockCount.Value, signalJob.BuyQuote.Ask * signalJob.BuyedStockCount.Value);
        var diffValue = (lastQuote.Bid * signalJob.BuyedStockCount.Value) - (signalJob.BuyQuote.Ask * signalJob.BuyedStockCount.Value);
        _logger.LogInformation("Sell {StockCount} x {SellPrice:N2}€ = {TotalValue:N2}€, Result: {diffValue:N2}€", 
            signalJob.BuyedStockCount, 
            lastQuote.Bid, 
            signalJob.BuyedStockCount * lastQuote.Bid, 
            diffValue);

        signalJob.SellStockAmount(lastQuote);

        return Task.CompletedTask;
    }
}
