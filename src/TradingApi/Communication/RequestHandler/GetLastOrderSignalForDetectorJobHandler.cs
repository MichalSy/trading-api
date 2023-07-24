using TradingApi.Communication.Request;
using TradingApi.Manager.OrderSignal;
using TradingApi.Manager.OrderSignal.Models;

namespace TradingApi.Communication.RequestHandler;

public class GetLastOrderSignalForDetectorJobHandler : IRequestHandler<GetLastOrderSignalForDetectorJobRequest, OrderSignalJob?>
{
    private readonly IOrderSignalManager _orderSignalManager;

    [SetsRequiredMembers]
    public GetLastOrderSignalForDetectorJobHandler(IOrderSignalManager orderSignalManager)
    {
        _orderSignalManager = orderSignalManager;
    }

    public async Task<OrderSignalJob?> Handle(GetLastOrderSignalForDetectorJobRequest request, CancellationToken cancellationToken)
    {
        return await _orderSignalManager.GetLastOrderSignalsForDetectorJobIdAsync(request.DetectorJobId);
    }
}
