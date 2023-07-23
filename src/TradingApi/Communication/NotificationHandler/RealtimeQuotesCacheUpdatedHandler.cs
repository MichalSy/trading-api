using TradingApi.Communication.Notification;
using TradingApi.Manager.OrderSignalDetector;

namespace TradingApi.Communication.NotificationHandler;

public class RealtimeQuotesCacheUpdatedHandler : INotificationHandler<RealtimeQuotesCacheUpdatedNotification>
{
    private readonly IOrderSignalDetectorManager _signalDetectorManager;

    [SetsRequiredMembers]
    public RealtimeQuotesCacheUpdatedHandler(IOrderSignalDetectorManager signalDetectorManager)
    {
        _signalDetectorManager = signalDetectorManager;
    }
    public async Task Handle(RealtimeQuotesCacheUpdatedNotification notification, CancellationToken cancellationToken)
    {
        await _signalDetectorManager.ExecuteDetectorsAsync(notification.LastQuote, notification.ChachedQuotes);
    }
}
