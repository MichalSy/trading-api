using TradingApi.Manager.Storage.SignalDetector.Models;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.Storage.OrderSignal.Models;

public class OrderSignalJob
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required Guid DetectorJobId { get; init; }
    public required OrderSignalBuySettings BuySettings { get; init; }
    public required OrderSignalSellSettings SellSettings { get; init; }
    public required string Isin { get; init; }

    public required RealtimeQuote BuyQuote { get; init; }
    public required int StockCount { get; init; }

    private RealtimeQuote? _sellQuote;
    public RealtimeQuote? SellQuote => _sellQuote;

    public DateTime CreatedDate { get; } = DateTime.UtcNow;

    private DateTime? _closedDate;
    public DateTime? ClosedDate => _closedDate;



    public bool IsClosed => _closedDate is { };


    public void SellStockAmount(RealtimeQuote realtimeQuote)
    {
        _sellQuote = realtimeQuote;
        _closedDate = DateTime.UtcNow;
    }
}
