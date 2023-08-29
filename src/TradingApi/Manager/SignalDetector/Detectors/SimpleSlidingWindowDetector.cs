using TradingApi.Manager.Storage.SignalDetector.Models;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.Storage.SignalDetector.Detectors;

public class SimpleSlidingWindowDetector : OrderSignalDetectorBase
{
    private readonly ILogger<SimpleSlidingWindowDetector> _logger;

    public override string DisplayName => "SlidingWindow (Simple)";

    [SetsRequiredMembers]
    public SimpleSlidingWindowDetector(ILogger<SimpleSlidingWindowDetector> logger, IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        _logger = logger;
    }

    protected override async Task ExecuteDetectionAsync(SignalDetectorJob orderSignalDetectorJob, RealtimeQuote lastQuote, IEnumerable<RealtimeQuote>? cachedQuotes)
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
            _logger.LogTrace("Start quote: {startQuoteBid} ({startQuoteTimestamp}), Last quote: {lastQuoteBid} ({lastQuoteTimestamp}) -> Difference percent: {differencePercent:N3}",
                lastQuoteBeforeWindow.Bid,
                lastQuoteBeforeWindow.Timestamp,
                lastQuote.Bid,
                lastQuote.Timestamp,
                differencePercent);

            // ignore difference under needDifferenceFromStart
            if (differencePercent < needDifferenceFromStart)
                return;

            // create new signal :)
            await SendCreaeOrderSignal(orderSignalDetectorJob, lastQuote);
        }
    }
}
