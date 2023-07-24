using TradingApi.Communication.Request;
using TradingApi.Repositories.ZeroRealtime;

namespace TradingApi.Communication.Commands;

public class SubscribeIsinHandler : IRequestHandler<SubscribeIsinRequest>
{
    private readonly IZeroRealtimeRepository _zeroRealtimeRepository;

    [SetsRequiredMembers]
    public SubscribeIsinHandler(IZeroRealtimeRepository zeroRealtimeRepository)
    {
        _zeroRealtimeRepository = zeroRealtimeRepository;
    }

    public async Task Handle(SubscribeIsinRequest request, CancellationToken cancellationToken)
    {
        await _zeroRealtimeRepository.SubscribeIsinAsync(request.Isin);
    }
}
