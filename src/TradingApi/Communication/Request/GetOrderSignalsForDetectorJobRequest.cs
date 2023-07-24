using TradingApi.Manager.OrderSignal.Models;

namespace TradingApi.Communication.Request;

public record GetOrderSignalsForDetectorJobRequest(
    Guid detectorJobId) : IRequest<IEnumerable<OrderSignalJob>>;
