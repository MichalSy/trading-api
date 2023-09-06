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
    public RealtimeQuote? SellQuote
    {
        get => _sellQuote;
        init => _sellQuote = value;
    }


    public DateTime CreatedDate { get; init; } = DateTime.UtcNow;

    private DateTime? _closedDate;
    public DateTime? ClosedDate => _closedDate;



    public bool IsClosed => _closedDate is { };

    public required decimal TotalBuyValueInEur { get; init; }

    private decimal? _totalSellValueInEur;
    public decimal? TotalSellValueInEur
    {
        get => _totalSellValueInEur;
        init => _totalSellValueInEur = value;
    }

    private decimal? _totalProfitInEur;
    public decimal? TotalProfitInEur
    {
        get => _totalProfitInEur;
        init => _totalProfitInEur = value;
    }

    public void SellStockAmount(RealtimeQuote realtimeQuote)
    {
        _sellQuote = realtimeQuote;
        _totalSellValueInEur = realtimeQuote.Bid * StockCount;
        _totalProfitInEur = _totalSellValueInEur - TotalBuyValueInEur;
        _closedDate = DateTime.UtcNow;
    }
}
