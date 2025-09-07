using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    /// <summary>
    /// Deterministic dummy prices for CI and offline runs.
    /// </summary>
    public sealed class DummyPriceFeed : IPriceFeed
    {
        public Task<IDictionary<string, decimal>> GetPricesAsync(IEnumerable<string> tickers, CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;
            var dict = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            foreach (var t in tickers.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                // Stable but slightly moving pseudo-price: hash + minute drift.
                var hash = Math.Abs(t.Aggregate(17, (acc, ch) => unchecked(acc * 31 + ch)));
                var basePrice = (hash % 300) + 50; // 50..349
                var drift = (now.Minute % 10) * 0.1m; // 0..0.9
                dict[t] = basePrice + drift;
            }
            return Task.FromResult<IDictionary<string, decimal>>(dict);
        }
    }
}
