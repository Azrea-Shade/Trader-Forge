using System.Linq;
using Infrastructure;
using System.Collections.Generic;

namespace Services.Engines
{
    public partial class AlertEngine
    {

        // Adapter: accept Infrastructure.AlertRow directly
        public IEnumerable<AlertEvaluation> Evaluate(IEnumerable<Infrastructure.AlertRow> rules, IDictionary<string,double> latestPrices)
        {
            var mapped = new List<AlertRow>();
            if (rules != null)
                foreach (var r in rules)
                    mapped.Add(new AlertRow(r.Id, r.Ticker, r.Above, r.Below));
            return Evaluate(mapped, latestPrices);
        }

        public IEnumerable<AlertEvaluation> Evaluate(IEnumerable<AlertRow> rules)
            => Evaluate(rules, new Dictionary<string,double>());

        public IEnumerable<AlertEvaluation> Evaluate(IEnumerable<AlertRow> rules, IDictionary<string,double> latestPrices)
        {
            var evals = new List<AlertEvaluation>();
            foreach (var rule in rules)
            {
                double price = 0;
                if (latestPrices != null && latestPrices.TryGetValue(rule.Ticker, out var p))
                    price = p;

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
