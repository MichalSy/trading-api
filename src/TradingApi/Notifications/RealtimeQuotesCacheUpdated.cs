using MediatR;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Notifications;

public record RealtimeQuotesCacheUpdated(RealtimeQuote LastQuote, IEnumerable<RealtimeQuote>? ChachedQuotes) : INotification;