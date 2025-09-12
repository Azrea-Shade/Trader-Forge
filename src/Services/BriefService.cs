using System;
using System.Linq;
using Domain;

namespace Services
{
    public sealed class BriefService : IBriefingService
    {
        private readonly IClock _clock;

        // Optional DI; default to SystemClock so tests can new BriefService() without args
        public BriefService(IClock? clock = null)
        {
            _clock = clock ?? new SystemClock();
        }

        public DailyBrief Generate(DateOnly date, string[] tickers)
        {
            tickers ??= Array.Empty<string>();
            var summary = tickers.Length == 0
                ? "No tickers selected."
                : $"Watchlist: {string.Join(", ", tickers)}";

            return new DailyBrief
            {
                Date = date,
                GeneratedAtLocal = _clock.Now,
                SummaryText = summary,
                Tickers = tickers
            };
        }
    }
}
