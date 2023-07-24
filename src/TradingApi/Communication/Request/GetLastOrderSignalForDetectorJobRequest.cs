using TradingApi.Manager.OrderSignal.Models;

namespace TradingApi.Communication.Request;

public record GetLastOrderSignalForDetectorJobRequest(
    Guid DetectorJobId) : IRequest<OrderSignalJob?>;
