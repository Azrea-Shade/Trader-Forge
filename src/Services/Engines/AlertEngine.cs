using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Engines
{
    public record AlertEvaluation(long Id, string Ticker, double Price, double? Above, double? Below, bool TriggeredAbove, bool TriggeredBelow);

    public static class AlertEngine
    {
        /// <summary>
        /// Evaluate threshold alerts given latest prices.
        /// </summary>
        public static IEnumerable<AlertEvaluation> Evaluate(
            IEnumerable<Infrastructure.AlertRow> rules,
            IDictionary<string, double> latestPrices)
        {
            foreach (var r in rules)
            {
                if (!latestPrices.TryGetValue(r.Ticker, out var p))
                    continue;

                bool hitAbove = r.Above.HasValue && p >= r.Above.Value;
                bool hitBelow = r.Below.HasValue && p <= r.Below.Value;

                if (!r.Enabled) { hitAbove = false; hitBelow = false; }

                yield return new AlertEvaluation(
                    r.Id, r.Ticker, p, r.Above, r.Below, hitAbove, hitBelow
                );
            }
        }
    }
}
