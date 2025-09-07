using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Engines
{
    public static class BriefingEngine
    {
        /// <summary>
        /// Compose a minimal morning brief from tickers and prices.
        /// </summary>
        public static IReadOnlyList<string> BuildBrief(
            IEnumerable<string> tickers,
            IDictionary<string, double> latestPrices,
            DateTime asOfUtc)
        {
            var lines = new List<string>
            {
                $"Good morning — {asOfUtc:yyyy-MM-dd HH:mm} UTC",
            };

            var selected = tickers
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Where(t => latestPrices.ContainsKey(t))
                .OrderBy(t => t)
                .ToList();

            if (selected.Count == 0)
            {
                lines.Add("No watchlist tickers found yet — add a few to get a richer brief.");
                return lines;
            }

            lines.Add("Watchlist snapshot:");
            foreach (var t in selected)
            {
                lines.Add($"• {t}: {latestPrices[t]:0.00}");
            }

            return lines;
        }
    }
}
