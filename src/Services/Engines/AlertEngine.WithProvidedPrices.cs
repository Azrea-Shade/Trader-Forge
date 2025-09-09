using System.Collections.Generic;
using Services.Feeds;

namespace Services.Engines
{
    public partial class AlertEngine
    {
        private readonly IPriceFeed _priceFeed;
        public AlertEngine(IPriceFeed priceFeed) => _priceFeed = priceFeed;

        // Back-compat when no prices supplied
        public IEnumerable<AlertEvaluation> Evaluate(IEnumerable<AlertRow> rules)
            => Evaluate(rules, new Dictionary<string,double>());

        // Preferred path: use provided latestPrices; fall back to feed
        public IEnumerable<AlertEvaluation> Evaluate(IEnumerable<AlertRow> rules, IDictionary<string,double> latestPrices)
        {
            var evals = new List<AlertEvaluation>();
            foreach (var rule in rules)
            {
                double price = 0;
                if (latestPrices != null && latestPrices.TryGetValue(rule.Ticker, out var p))
                    price = p;
                else if (_priceFeed != null)
                    price = _priceFeed.GetPriceAsync(rule.Ticker, null).GetAwaiter().GetResult();

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
