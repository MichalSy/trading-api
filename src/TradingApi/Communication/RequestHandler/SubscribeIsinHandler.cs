using TradingApi.Repositories.ZeroRealtime;

namespace TradingApi.Communication.Commands;

public record SubscribeIsinCommand(string Isin)
    : IRequest;

public class SubscribeIsinHandler : IRequestHandler<SubscribeIsinCommand>
{
    private readonly IZeroRealtimeRepository _zeroRealtimeRepository;

    [SetsRequiredMembers]
    public SubscribeIsinHandler(IZeroRealtimeRepository zeroRealtimeRepository)
    {
        _zeroRealtimeRepository = zeroRealtimeRepository;
    }

    public async Task Handle(SubscribeIsinCommand request, CancellationToken cancellationToken)
    {
        await _zeroRealtimeRepository.SubscribeIsinAsync(request.Isin);
    }
}
