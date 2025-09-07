using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public static class PriceFeedExtensions
    {
        /// <summary>
        /// Compatibility helper: provide an async bulk-price API even if IPriceFeed only has LastPrice(string).
        /// </summary>
        public static Task<IDictionary<string, double>> GetPricesAsync(this IPriceFeed feed, IEnumerable<string> tickers, CancellationToken ct = default)
        {
            var result = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
            if (feed is null) return Task.FromResult<IDictionary<string, double>>(result);
            if (tickers is null) return Task.FromResult<IDictionary<string, double>>(result);

            foreach (var raw in tickers)
            {
                if (ct.IsCancellationRequested) break;
                if (string.IsNullOrWhiteSpace(raw)) continue;
                var t = raw.Trim();
                if (result.ContainsKey(t)) continue;
                result[t] = feed.LastPrice(t);
            }
            return Task.FromResult<IDictionary<string, double>>(result);
        }
    }
}
