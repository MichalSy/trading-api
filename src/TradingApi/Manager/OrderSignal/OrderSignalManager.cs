using System.Collections.Concurrent;
using MongoDB.Bson;
using TradingApi.Manager.OrderSignalDetector.Models;
using TradingApi.Manager.Storage.OrderSignal.Models;
using TradingApi.Manager.Storage.SignalDetector.Models;
using TradingApi.Repositories.Storages;
using TradingApi.Repositories.Storages.Models;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.Storage.OrderSignal;

public class OrderSignalManager : IOrderSignalManager
{
    private ConcurrentBag<OrderSignalJob> _orderSignals = new();
    private readonly ILogger<OrderSignalManager> _logger;
    private readonly IOrderSignalStorage _orderSignalStorage;

    [SetsRequiredMembers]
    public OrderSignalManager(ILogger<OrderSignalManager> logger, IOrderSignalStorage orderSignalStorage)
    {
        _logger = logger;
        _orderSignalStorage = orderSignalStorage;
    }

    public async Task StartAsync(CancellationToken stoppingToken)
    {
        _ = CleanupClosedJobsIntervalTask(stoppingToken);

        await LoadAllOrderSignalsFromDatabaseAsync();
    }

    private async Task LoadAllOrderSignalsFromDatabaseAsync()
    {
        var dbData = (await _orderSignalStorage.GetOrderSignalsAsync())?.Select(i => i.ToDTO());
        if (dbData is null)
            return;

        _orderSignals = new ConcurrentBag<OrderSignalJob>(dbData);
    }

    public async Task<bool> CreateOrderSignalFromDetectorJobAsync(SignalDetectorJob detectorJob, RealtimeQuote lastQuote)
    {
        return await BuyStockAsync(detectorJob, lastQuote);
    }

    public Task<OrderSignalJob?> GetLastOrderSignalsForDetectorJobIdAsync(Guid detectorJobid)
    {
        return Task.FromResult(_orderSignals.Where(o => o.DetectorJobId.Equals(detectorJobid)).OrderByDescending(o => o.CreatedDate).FirstOrDefault());
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
        while (!stoppingToken.IsCancellationRequested)
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

        if (differenceBuyPriceAndLastBidPrice > signalJob.SellSettings.DifferencePositiveInPercent)
        {
            await SellStockAsync(signalJob, lastQuote);
            return;
        }

        if (differenceBuyPriceAndLastBidPrice < signalJob.SellSettings.DifferenceNegativeInPercent)
        {
            await SellStockAsync(signalJob, lastQuote);
            return;
        }
    }

    private async Task<bool> BuyStockAsync(SignalDetectorJob detectorJob, RealtimeQuote lastQuote)
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
            return false;

        var newOrderSignalDBO =  new OrderSignalEntityDBO
        {
            DetectorJobId = detectorJob.Id,
            Isin = detectorJob.Isin,
            StockCount = stockAmount,
            BuySettings = new OrderSignalBuySettingsDBO
            {
                CoolDownAfterLastSellInSecs = detectorJob.OrderSignalSettings.BuySettings.CoolDownAfterLastSellInSecs,
                RoundUpValueInEur = detectorJob.OrderSignalSettings.BuySettings.RoundUpValueInEur,
                StockCount = detectorJob.OrderSignalSettings.BuySettings.StockCount,
                ValueInEur = detectorJob.OrderSignalSettings.BuySettings.ValueInEur
            },
            BuyQuote = lastQuote.ToDBO(),
            SellSettings = new OrderSignalSellSettingsDBO
            {
                DifferenceNegativeInPercent = detectorJob.OrderSignalSettings.SellSettings.DifferenceNegativeInPercent,
                DifferencePositiveInPercent = detectorJob.OrderSignalSettings.SellSettings.DifferencePositiveInPercent,
                MaxDuration = detectorJob.OrderSignalSettings.SellSettings.MaxDuration
            },
            TotalBuyValueInEur = stockAmount * lastQuote.Ask
        };

        var newJobDBO = await _orderSignalStorage.CreateOrUpdateOrderSignalAsync(newOrderSignalDBO);
        _orderSignals.Add(newJobDBO.ToDTO());

        _logger.LogInformation("Buy {StockCount} x {BuyPrice:N2}€ = {TotalValue:N2}€", stockAmount, lastQuote.Ask, stockAmount * lastQuote.Ask);
        return true;
    }

    private async Task SellStockAsync(OrderSignalJob signalJob, RealtimeQuote lastQuote)
    {
        var diffValue = (lastQuote.Bid * signalJob.StockCount) - (signalJob.BuyQuote.Ask * signalJob.StockCount);

        signalJob.SellStockAmount(lastQuote);
        await _orderSignalStorage.CreateOrUpdateOrderSignalAsync(signalJob.ToDBO());

        _logger.LogInformation("Sell {StockCount} x {SellPrice:N2}€ = {TotalValue:N2}€, Result: {diffValue:N2}€",
            signalJob.StockCount,
            lastQuote.Bid,
            signalJob.StockCount * lastQuote.Bid,
            diffValue);
    }
}
