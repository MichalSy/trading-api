using System.Text.Json.Nodes;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.OrderSignalDetector.Detectors;

public class SimpleSlidingWindowDetector : OrderSignalDetectorBase
{
    protected override string DisplayName => "SlidingWindow (Simple)";

    protected override Task DetectAsync(Dictionary<string, JsonValue> settings, RealtimeQuote lastQuote, IEnumerable<RealtimeQuote>? cachedQuotes)
    {

        return Task.CompletedTask;
    }
}
