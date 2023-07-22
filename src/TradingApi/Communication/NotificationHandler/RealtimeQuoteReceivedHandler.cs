using MediatR;
using System.Diagnostics.CodeAnalysis;
using TradingApi.Manager.RealtimeQuotesStorage;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Communication.NotificationHandler;

public record RealtimeQuoteReceived(RealtimeQuote Quote) : INotification;

public class RealtimeQuoteReceivedHandler : INotificationHandler<RealtimeQuoteReceived>
{
    private readonly IRealtimeQuotesStorageManager _quotesStorageManager;

    [SetsRequiredMembers]
    public RealtimeQuoteReceivedHandler(IRealtimeQuotesStorageManager quotesStorageManager)
    {
        _quotesStorageManager = quotesStorageManager;
    }

    public async Task Handle(RealtimeQuoteReceived notification, CancellationToken cancellationToken)
    {
        await _quotesStorageManager.CaptureQuoteAsync(notification.Quote);
    }
}
