using TradingApi.Communication.Request;
using TradingApi.Manager.TradingStorage;
using TradingApi.Manager.TradingStorage.Models;
using TradingApi.Repositories.ZeroRealtime;

namespace TradingApi.Communication.Commands;

public class SubscribeIsinHandler : IRequestHandler<SubscribeIsinRequest>
{
    private readonly IZeroRealtimeRepository _zeroRealtimeRepository;
    private readonly ITradingStorageManager _storageManager;

    [SetsRequiredMembers]
    public SubscribeIsinHandler(IZeroRealtimeRepository zeroRealtimeRepository, ITradingStorageManager storageManager)
    {
        _zeroRealtimeRepository = zeroRealtimeRepository;
        _storageManager = storageManager;
    }

    public async Task Handle(SubscribeIsinRequest request, CancellationToken cancellationToken)
    {
        await _storageManager.CreateOrUpdateInstrumentInDatabaseAsync(new InstrumentDTO { Isin = request.Isin });
        await _zeroRealtimeRepository.SubscribeIsinAsync(request.Isin);
    }
}
