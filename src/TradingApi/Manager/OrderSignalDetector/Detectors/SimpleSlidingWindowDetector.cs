﻿using MediatR;
using TradingApi.Communication.Request;
using TradingApi.Manager.OrderSignalDetector.Models;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.OrderSignalDetector.Detectors;

public class SimpleSlidingWindowDetector : OrderSignalDetectorBase
{
    private readonly ILogger<SimpleSlidingWindowDetector> _logger;
    private readonly ISender _sender;

    public override string DisplayName => "SlidingWindow (Simple)";

    public SimpleSlidingWindowDetector(ILogger<SimpleSlidingWindowDetector> logger, ISender sender)
    {
        _logger = logger;
        _sender = sender;
    }

    public override async Task DetectAsync(OrderSignalDetectorJob orderSignalDetectorJob, RealtimeQuote lastQuote, IEnumerable<RealtimeQuote>? cachedQuotes)
    {
        var windowTime = orderSignalDetectorJob.GetDetectorSettingValue("WindowTimeInSecs", 30);
        var needDifferenceFromStart = orderSignalDetectorJob.GetDetectorSettingValue("BidDifferenceFromWindowStartInPercent", 5f);

        var windowStart = DateTime.UtcNow.AddSeconds(-windowTime);

        // get last quote before window
        var lastQuoteBeforeWindow = cachedQuotes?.Where(q => q.Timestamp < windowStart).OrderByDescending(q => q.Timestamp).FirstOrDefault();

        // difference percent bid value between last quote before window and last quote
        if (lastQuoteBeforeWindow is { })
        {
            var differencePercent = (float)(lastQuote.Bid / lastQuoteBeforeWindow.Bid * 100 - 100);
            _logger.LogTrace("Start quote: {startQuoteBid} ({startQuoteTimestamp}), Last quote: {lastQuoteBid} ({lastQuoteTimestamp}) -> Difference percent: {differencePercent:N3}",
                lastQuoteBeforeWindow.Bid,
                lastQuoteBeforeWindow.Timestamp,
                lastQuote.Bid,
                lastQuote.Timestamp,
                differencePercent);

            // ignore difference under needDifferenceFromStart
            if (differencePercent < needDifferenceFromStart)
                return;

            // ignore signal if another signal exists for this job
            var jobs = await _sender.Send(new GetOrderSignalsForDetectorJobRequest(orderSignalDetectorJob.Id));
            if (jobs?.Any() == true)
            {
                _logger.LogTrace("Can't create new signal, another one is still running");
                return;
            }
            
            // create new signal :)
            _ = _sender.Send(new CreateOrderSignalRequest(orderSignalDetectorJob, lastQuote));
        }
    }
}
