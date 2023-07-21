using MediatR;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Notifications;

public record RealtimeQuotesCacheUpdated(IEnumerable<RealtimeQuote> lastQuotes) : INotification;