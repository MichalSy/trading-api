using TradingApi.Communication.Notification;
using TradingApi.Manager.RealtimeQuotesStorage;

namespace TradingApi.Communication.NotificationHandler;

public class RealtimeQuoteReceivedStorageHandler : INotificationHandler<RealtimeQuoteReceivedNotification>
{
    private readonly IRealtimeQuotesStorageManager _quotesStorageManager;

    [SetsRequiredMembers]
    public RealtimeQuoteReceivedStorageHandler(IRealtimeQuotesStorageManager quotesStorageManager)
    {
        _quotesStorageManager = quotesStorageManager;
    }

    public async Task Handle(RealtimeQuoteReceivedNotification notification, CancellationToken cancellationToken)
    {
        await _quotesStorageManager.CaptureQuoteAsync(notification.Quote);
    }
}
