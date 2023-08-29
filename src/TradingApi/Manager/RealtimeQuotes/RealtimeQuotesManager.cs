using TradingApi.Manager.Storage.InstrumentStorage;
using TradingApi.Manager.Storage.InstrumentStorage.Models;
using TradingApi.Manager.Storage.OrderSignal;
using TradingApi.Manager.Storage.OrderSignalDetector;
using TradingApi.Repositories.ZeroRealtime;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.RealtimeQuotes;

public class RealtimeQuotesManager : IRealtimeQuotesManager
{
    private readonly ILogger<RealtimeQuotesManager> _logger;
    private readonly IZeroRealtimeRepository _zeroRealtimeRepository;
    private readonly IInstrumentStorageManager _instrumentStorageManager;
    private readonly IOrderSignalManager _orderSignalManager;
    private readonly IOrderSignalDetectorManager _orderSignalDetectorManager;
    private readonly Dictionary<string, List<RealtimeQuote>> _cacheQuotes = new();

    [SetsRequiredMembers]
    public RealtimeQuotesManager(
        ILogger<RealtimeQuotesManager> logger,
        IZeroRealtimeRepository zeroRealtimeRepository, 
        IInstrumentStorageManager instrumentStorageManager,
        IOrderSignalManager orderSignalManager,
        IOrderSignalDetectorManager orderSignalDetectorManager)
    {
        _instrumentStorageManager = instrumentStorageManager;
        _orderSignalManager = orderSignalManager;
        _orderSignalDetectorManager = orderSignalDetectorManager;
        _logger = logger;
        _zeroRealtimeRepository = zeroRealtimeRepository;
        _zeroRealtimeRepository.SubscribeQuoteChange(RealtimeQuoteChangedReceived);
    }

    public async Task SubscribeIsinAsync(string isin)
    {
        await _instrumentStorageManager.CreateOrUpdateInstrumentAsync(new InstrumentDTO { Isin = isin });
        await _zeroRealtimeRepository.SubscribeIsinAsync(isin);
    }

    public async void RealtimeQuoteChangedReceived(RealtimeQuote quote)
    {
        if (quote.Timestamp.Date != DateTime.UtcNow.Date)
            return;

        await SaveQuoteAsync(quote);
        await _orderSignalManager.UpdateOrderSignalsAsync(quote);
    }

    private async Task SaveQuoteAsync(RealtimeQuote quote)
    {
        _logger.LogInformation("Get Quote from isin: {isin}, ask: {ask} bid: {bid}", quote.Isin, quote.Ask, quote.Bid);

        if (!_cacheQuotes.TryGetValue(quote.Isin, out var currentQuotes))
        {
            var newList = new List<RealtimeQuote> { quote };
            _cacheQuotes.Add(quote.Isin, newList);
            await _orderSignalDetectorManager.ExecuteDetectorsAsync(quote, null);
        }
        else
        {
            currentQuotes.RemoveAll(q => q.Timestamp < DateTime.UtcNow.AddHours(-2));
            await _orderSignalDetectorManager.ExecuteDetectorsAsync(quote, currentQuotes.ToArray());
            currentQuotes.Add(quote);
        }

        //var a = await _tradingStorageManager.SaveQuoteInDatabaseAsync(quote);
    }
}
