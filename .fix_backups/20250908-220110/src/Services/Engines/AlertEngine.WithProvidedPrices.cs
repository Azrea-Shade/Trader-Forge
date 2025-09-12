using System.Collections.Generic;

namespace Services.Engines
{
    public static partial class AlertEngine
    {
        public static IEnumerable<AlertEvaluation> EvaluateWithPrices(
            IEnumerable<AlertRow> rules,
            IDictionary<string, double> latestPrices)
        {
            var evals = new List<AlertEvaluation>();
            foreach (var rule in rules)
            {
                double price = 0;
                if (latestPrices != null) latestPrices.TryGetValue(rule.Ticker, out price);

                var e = new AlertEvaluation
                {
                    Id = rule.Id,
                    Ticker = rule.Ticker,
                    Price = price,
                    Above = rule.Above,
                    Below = rule.Below,
                    TriggeredAbove = rule.Above.HasValue && price >= rule.Above.Value,
                    TriggeredBelow = rule.Below.HasValue && price <= rule.Below.Value
                };
                evals.Add(e);
            }
            return evals;
        }
    }
}
