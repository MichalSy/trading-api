using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Communication.Notification;

public record RealtimeQuoteReceivedNotification(RealtimeQuote Quote) : INotification;
