using TradingApi.Communication.Request;
using TradingApi.Manager.OrderSignal;
using TradingApi.Manager.OrderSignal.Models;

namespace TradingApi.Communication.RequestHandler;

public class GetOrderSignalsForDetectorJobHandler : IRequestHandler<GetOrderSignalsForDetectorJobRequest, IEnumerable<OrderSignalJob>>
{
    private readonly IOrderSignalManager _orderSignalManager;

    [SetsRequiredMembers]
    public GetOrderSignalsForDetectorJobHandler(IOrderSignalManager orderSignalManager)
    {
        _orderSignalManager = orderSignalManager;
    }

    public async Task<IEnumerable<OrderSignalJob>> Handle(GetOrderSignalsForDetectorJobRequest request, CancellationToken cancellationToken)
    {
        return await _orderSignalManager.GetActiveOrderSignalsForDetectorJobIdAsync(request.detectorJobId);
    }
}
