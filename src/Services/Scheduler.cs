using System;
using Domain;

namespace Services
{
    /// <summary>
    /// Minimal, test-friendly scheduler shim. Uses IClock for time, IBriefingService to build the brief,
    /// and INotifier to deliver it. Defaults keep CI green without Windows toast APIs.
    /// </summary>
    public sealed class Scheduler
    {
        private readonly IClock _clock;
        private readonly IBriefingService _briefing;
        private readonly INotifier _notifier;

        public Scheduler(IClock? clock = null, IBriefingService? briefing = null, INotifier? notifier = null)
        {
            _clock    = clock    ?? new SystemClock();
            _briefing = briefing ?? new BriefService(_clock);
            _notifier = notifier ?? new LogNotifier();
        }

        /// <summary>Runs the daily brief once for the given date/tickers and sends a notification.</summary>
        public void RunDailyBrief(DateOnly date, string[]? tickers)
        {
            var list  = tickers ?? Array.Empty<string>();
            DailyBrief brief = _briefing.Generate(date, list);
            _notifier.Notify("Trader Forge Tracker", brief.SummaryText);
        }
    }
}
