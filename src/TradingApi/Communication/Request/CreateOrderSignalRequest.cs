using TradingApi.Manager.OrderSignalDetector.Models;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Communication.Request;

public record CreateOrderSignalRequest(OrderSignalDetectorJob OrderSignalDetectorJob, RealtimeQuote LastQuote)
    : IRequest;