using TradingApi.Manager.Storage.SignalDetector.Models;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.Storage.OrderSignal.Models;

public class OrderSignalJob
{
    public required SignalDetectorJob DetectorJob { get; init; }

    private RealtimeQuote? _buyQuote;
    public RealtimeQuote? BuyQuote => _buyQuote;

    private int? _buyedStockCount = 0;
    public int? BuyedStockCount => _buyedStockCount;

    private RealtimeQuote? _sellQuote;
    public RealtimeQuote? SellQuote => _sellQuote;

    public string Isin => DetectorJob.Isin;

    public DateTime CreatedDate { get; } = DateTime.UtcNow;

    private DateTime? _closedDate;
    public DateTime? ClosedDate => _closedDate;

    

    public bool IsClosed => _closedDate is { };

    public void BuyStockCount(RealtimeQuote realtimeQuote, int stockCount = 1)
    {
        _buyQuote = realtimeQuote;
        _buyedStockCount = stockCount;
    }

    public void SellStockAmount(RealtimeQuote realtimeQuote)
    {
        _sellQuote = realtimeQuote;
        _closedDate = DateTime.UtcNow;
    }
}
