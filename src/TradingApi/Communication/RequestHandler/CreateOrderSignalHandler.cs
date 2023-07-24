using TradingApi.Communication.Request;
using TradingApi.Manager.OrderSignal;

namespace TradingApi.Communication.RequestHandler;

public class CreateOrderSignalHandler : IRequestHandler<CreateOrderSignalRequest>
{
    private readonly IOrderSignalManager _orderSignalManager;

    [SetsRequiredMembers]
    public CreateOrderSignalHandler(IOrderSignalManager orderSignalManager)
    {
        _orderSignalManager = orderSignalManager;
    }

    public Task Handle(CreateOrderSignalRequest request, CancellationToken cancellationToken)
    {
        return _orderSignalManager.CreateOrderSignalFromDetectorJobAsync(request.OrderSignalDetectorJob, request.LastQuote);
    }
}
