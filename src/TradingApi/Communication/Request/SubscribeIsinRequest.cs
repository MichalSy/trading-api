namespace TradingApi.Communication.Request;

public record SubscribeIsinRequest(string Isin)
    : IRequest;
