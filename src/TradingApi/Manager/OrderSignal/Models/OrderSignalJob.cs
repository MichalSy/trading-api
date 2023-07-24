using TradingApi.Manager.OrderSignalDetector.Models;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.OrderSignal.Models;

public class OrderSignalJob
{
    public required OrderSignalDetectorJob DetectorJob { get; init; }

    private RealtimeQuote? _buyQuote;
    public RealtimeQuote? BuyQuote => _buyQuote;

    private int? _buyedStockCount = 0;
    public int? BuyedStockCount => _buyedStockCount;

    private RealtimeQuote? _sellQuote;
    public RealtimeQuote? SellQuote => _sellQuote;

    public string Isin => DetectorJob.Isin;

    public DateTime OrderDate { get; } = DateTime.UtcNow;

    private bool _isClosed = false;
    public bool IsClosed => _isClosed;

    public void BuyStockCount(RealtimeQuote realtimeQuote, int stockCount = 1)
    {
        _buyQuote = realtimeQuote;
        _buyedStockCount = stockCount;
    }

    public void SellStockAmount(RealtimeQuote realtimeQuote)
    {
        _sellQuote = realtimeQuote;
        _isClosed = true;
    }
}
