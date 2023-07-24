using TradingApi.Communication.Request;
using TradingApi.Manager.OrderSignalDetector.Models;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.OrderSignalDetector.Detectors;

public abstract class OrderSignalDetectorBase : IOrderSignalDetector
{
    private readonly ISender _sender;

    public virtual string Name => GetType().Name.Replace("Detector", string.Empty);

    public virtual string DisplayName => Name;

    [SetsRequiredMembers]
    public OrderSignalDetectorBase(ISender sender)
    {
        _sender = sender;
    }

    public async Task StartDetectAsync(OrderSignalDetectorJob orderSignalJob, RealtimeQuote lastQuote, IEnumerable<RealtimeQuote>? cachedQuotes)
    {
        if (!await CanExecuteDetectionAsync(orderSignalJob, lastQuote, cachedQuotes))
        {
            return;
        }

        await ExecuteDetectionAsync(orderSignalJob, lastQuote, cachedQuotes);
    }

    protected virtual async Task<bool> CanExecuteDetectionAsync(OrderSignalDetectorJob orderSignalDetectorJob, RealtimeQuote lastQuote, IEnumerable<RealtimeQuote>? cachedQuotes)
    {
        var lastSignal = await _sender.Send(new GetLastOrderSignalForDetectorJobRequest(orderSignalDetectorJob.Id));
        if (lastSignal is { })
        {
            // allow only one active signal
            if (!lastSignal.IsClosed)
                return false;

            // check cooldown after last sell
            if (lastSignal.ClosedDate.HasValue
                && lastSignal.ClosedDate.Value.AddSeconds(orderSignalDetectorJob.OrderSignalSettings.BuySettings.CoolDownAfterLastSellInSecs) > DateTime.UtcNow)
                return false;
        }
        return true;
    }

    protected abstract Task ExecuteDetectionAsync(OrderSignalDetectorJob orderSignalJob, RealtimeQuote lastQuote, IEnumerable<RealtimeQuote>? cachedQuotes);

    protected async Task SendCreaeOrderSignal(OrderSignalDetectorJob orderSignalDetectorJob, RealtimeQuote lastQuote)
    {
        await _sender.Send(new CreateOrderSignalRequest(orderSignalDetectorJob, lastQuote));
    }


}
