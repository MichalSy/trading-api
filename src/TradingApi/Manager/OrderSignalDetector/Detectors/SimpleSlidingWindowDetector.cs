using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.OrderSignalDetector.Detectors;

public class SimpleSlidingWindowDetector : OrderSignalDetectorBase
{
    public override string DisplayName => "SlidingWindow (Simple)";

    public override Task DetectAsync(Dictionary<string, object> settings, RealtimeQuote lastQuote, IEnumerable<RealtimeQuote>? cachedQuotes)
    {

        return Task.CompletedTask;
    }
}
