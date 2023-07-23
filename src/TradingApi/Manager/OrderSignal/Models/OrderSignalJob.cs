using TradingApi.Manager.OrderSignalDetector.Models;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.OrderSignal.Models;

public class OrderSignalJob
{
    public required OrderSignalDetectorJob DetectorJob { get; init; }

    public required RealtimeQuote BuyQuote { get; init; }

    public string Isin => DetectorJob.Isin;

    public DateTime OrderDate { get; } = DateTime.UtcNow;

    public bool IsClosed { get; set; }

    
}
