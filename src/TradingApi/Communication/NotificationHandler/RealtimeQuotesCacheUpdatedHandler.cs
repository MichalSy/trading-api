using MediatR;
using System.Diagnostics.CodeAnalysis;
using TradingApi.Manager.OrderSignalDetector;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Communication.NotificationHandler;

public record RealtimeQuotesCacheUpdated(RealtimeQuote LastQuote, IEnumerable<RealtimeQuote>? ChachedQuotes) : INotification;

public class RealtimeQuotesCacheUpdatedHandler : INotificationHandler<RealtimeQuotesCacheUpdated>
{
    private readonly IOrderSignalDetectorManager _signalDetectorManager;

    [SetsRequiredMembers]
    public RealtimeQuotesCacheUpdatedHandler(IOrderSignalDetectorManager signalDetectorManager)
    {
        _signalDetectorManager = signalDetectorManager;
    }
    public async Task Handle(RealtimeQuotesCacheUpdated notification, CancellationToken cancellationToken)
    {
        await _signalDetectorManager.ExecuteDetecotsAsync(notification.LastQuote, notification.ChachedQuotes);
    }
}
