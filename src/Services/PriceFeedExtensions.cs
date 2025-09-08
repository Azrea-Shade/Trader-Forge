using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public static class PriceFeedExtensions
    {
        /// <summary>
        /// Get prices, but never throw. Missing/failed tickers map to null.
        /// </summary>
        public static async Task<IDictionary<string, double?>> SafeGetPricesAsync(
            this IPriceFeed feed,
            IEnumerable<string> tickers,
            CancellationToken ct = default)
        {
            try
            {
                return await feed.GetPricesAsync(tickers, ct).ConfigureAwait(false);
            }
            catch
            {
                var list = (tickers ?? Array.Empty<string>()).Distinct(StringComparer.OrdinalIgnoreCase);
                return list.ToDictionary(t => t, _ => (double?)null, StringComparer.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Convenience: return decimal? instead of double? for callers that prefer decimal.
        /// </summary>
        public static async Task<IDictionary<string, decimal?>> SafeGetPricesDecimalAsync(
            this IPriceFeed feed,
            IEnumerable<string> tickers,
            CancellationToken ct = default)
        {
            var d = await feed.SafeGetPricesAsync(tickers, ct).ConfigureAwait(false);
            return d.ToDictionary(kv => kv.Key,
                                  kv => kv.Value.HasValue ? (decimal?)Convert.ToDecimal(kv.Value.Value) : null,
                                  StringComparer.OrdinalIgnoreCase);
        }
    }
}
