using TradingApi.Communication.Request;
using TradingApi.Manager.OrderSignal;

namespace TradingApi.Communication.RequestHandler;

public class CreateOrderSignalHandler : IRequestHandler<CreateOrderSignalRequest, bool>
{
    private readonly IOrderSignalManager _orderSignalManager;

    [SetsRequiredMembers]
    public CreateOrderSignalHandler(IOrderSignalManager orderSignalManager)
    {
        _orderSignalManager = orderSignalManager;
    }

    public Task<bool> Handle(CreateOrderSignalRequest request, CancellationToken cancellationToken)
    {
        return _orderSignalManager.CreateOrderSignalFromDetectorJobAsync(request.OrderSignalDetectorJob, request.LastQuote);
    }
}
