using MediatR;
using TradingApi.Communication.RequestHandler;
using TradingApi.Manager.OrderSignalDetector.Models;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.OrderSignalDetector.Detectors;

public class SimpleSlidingWindowDetector : OrderSignalDetectorBase
{
    private readonly ISender _sender;

    public override string DisplayName => "SlidingWindow (Simple)";

    public SimpleSlidingWindowDetector(ISender sender)
    {
        _sender = sender;
    }

    public override Task DetectAsync(OrderSignalDetectorJob orderSignalDetectorJob, RealtimeQuote lastQuote, IEnumerable<RealtimeQuote>? cachedQuotes)
    {
        var windowTime = orderSignalDetectorJob.GetDetectorSettingValue("WindowTimeInSecs", 30);
        var needDifferenceFromStart = orderSignalDetectorJob.GetDetectorSettingValue("BidDifferenceFromWindowStartInPercent", 5f);

        var windowStart = DateTime.UtcNow.AddSeconds(-windowTime);

        // get last quote before window
        var lastQuoteBeforeWindow = cachedQuotes?.Where(q => q.Timestamp < windowStart).OrderByDescending(q => q.Timestamp).FirstOrDefault();

        // difference percent bid value between last quote before window and last quote
        if (lastQuoteBeforeWindow is { })
        {
            var differencePercent = (float)(lastQuote.Bid / lastQuoteBeforeWindow.Bid * 100 - 100);

            if (differencePercent >= needDifferenceFromStart)
            {
                _sender.Send(new CreateOrderSignalCommand(orderSignalDetectorJob, lastQuote));
            }
        }

        return Task.CompletedTask;
    }
}
