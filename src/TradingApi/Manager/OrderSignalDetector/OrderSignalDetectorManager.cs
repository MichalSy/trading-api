using MediatR;
using TradingApi.Notifications;

namespace TradingApi.Manager.OrderSignalDetector;

public class OrderSignalDetectorManager : IOrderSignalDetectorManager, INotificationHandler<RealtimeQuotesCacheUpdated>
{
    public OrderSignalDetectorManager()
    {
            
    }

    public Task Handle(RealtimeQuotesCacheUpdated notification, CancellationToken cancellationToken)
    {

        return Task.CompletedTask;
    }
}
