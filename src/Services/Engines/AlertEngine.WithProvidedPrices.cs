using System.Collections.Generic;

namespace Services.Engines
{
    public static partial class AlertEngine
    {
        public static IEnumerable<AlertEvaluation> EvaluateWithPrices(
            IEnumerable<dynamic> rules,
            IDictionary<string, double> latestPrices)
        {
            var evals = new List<AlertEvaluation>();
            foreach (var rule in rules)
            {
                string ticker = (string)rule.Ticker;
                long id = (long)rule.Id;
                double? above = (double?)rule.Above;
                double? below = (double?)rule.Below;

                double price = 0;
                if (latestPrices != null && latestPrices.TryGetValue(ticker, out var p))
                    price = p;

                var e = new AlertEvaluation
                {
                    Id = id,
                    Ticker = ticker,
                    Price = price,
                    Above = above,
                    Below = below,
                    TriggeredAbove = above.HasValue && price >= above.Value,
                    TriggeredBelow = below.HasValue && price <= below.Value
                };
                evals.Add(e);
            }
            return evals;
        }
    }
}
