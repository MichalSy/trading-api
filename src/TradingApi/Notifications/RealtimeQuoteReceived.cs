using MediatR;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Notifications;

public record RealtimeQuoteReceived(RealtimeQuote Quote) : INotification;
