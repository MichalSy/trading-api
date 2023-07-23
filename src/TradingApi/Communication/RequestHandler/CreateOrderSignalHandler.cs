using MediatR;
using System.Diagnostics.CodeAnalysis;
using TradingApi.Manager.OrderSignal;
using TradingApi.Manager.OrderSignalDetector.Models;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Communication.RequestHandler;


public record CreateOrderSignalCommand(OrderSignalDetectorJob OrderSignalDetectorJob, RealtimeQuote LastQuote)
    : IRequest;

public class CreateOrderSignalHandler : IRequestHandler<CreateOrderSignalCommand>
{
    private readonly IOrderSignalManager _orderSignalManager;

    [SetsRequiredMembers]
    public CreateOrderSignalHandler(IOrderSignalManager orderSignalManager)
    {
        _orderSignalManager = orderSignalManager;
    }

    public Task Handle(CreateOrderSignalCommand request, CancellationToken cancellationToken)
    {
        return _orderSignalManager.CreateOrderSignalFromDetectorJobAsync(request.OrderSignalDetectorJob, request.LastQuote);
    }
}
