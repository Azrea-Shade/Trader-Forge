using System;
using System.Linq;
using Domain;

namespace Services
{
    // Implements IBriefingService for CI/tests. No external deps.
    public sealed class BriefingService : IBriefingService
    {
        public DailyBrief Generate(DateOnly date, string[] tickers)
        {
            tickers ??= Array.Empty<string>();

            var summary = tickers.Length == 0
                ? "No tickers selected."
                : $"Watchlist: {string.Join(", ", tickers)}";

            return new DailyBrief
            {
                Date = date,
                GeneratedAtLocal = DateTimeOffset.Now, // keep it simple for CI
                SummaryText = summary,
                Tickers = tickers
            };
        }
    }
}
