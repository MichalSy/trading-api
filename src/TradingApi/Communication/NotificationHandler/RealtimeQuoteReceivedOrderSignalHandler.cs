using TradingApi.Communication.Notification;
using TradingApi.Manager.OrderSignal;

namespace TradingApi.Communication.NotificationHandler;

public class RealtimeQuoteReceivedOrderSignalHandler : INotificationHandler<RealtimeQuoteReceivedNotification>
{
    private readonly IOrderSignalManager _orderSignalManager;

    [SetsRequiredMembers]
    public RealtimeQuoteReceivedOrderSignalHandler(IOrderSignalManager orderSignalManager)
    {
        _orderSignalManager = orderSignalManager;
    }

    public async Task Handle(RealtimeQuoteReceivedNotification notification, CancellationToken cancellationToken)
    {
        if (notification.Quote.Timestamp.Date != DateTime.UtcNow.Date)
            return;

        await _orderSignalManager.UpdateOrderSignalsAsync(notification.Quote);
    }
}
