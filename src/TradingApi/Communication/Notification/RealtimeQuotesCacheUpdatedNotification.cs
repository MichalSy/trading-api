using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Communication.Notification;

public record RealtimeQuotesCacheUpdatedNotification(RealtimeQuote LastQuote, IEnumerable<RealtimeQuote>? ChachedQuotes) : INotification;