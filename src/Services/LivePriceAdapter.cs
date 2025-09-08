using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    /// <summary>
    /// Minimal adapter that satisfies IPriceFeed. It returns null prices by default
    /// (safe for offline/demo). Wire a real source later by changing the body.
    /// </summary>
    public sealed class LivePriceAdapter : IPriceFeed
    {
        public Task<IDictionary<string, double?>> GetPricesAsync(IEnumerable<string> tickers, CancellationToken ct = default)
        {
            // Normalize and return a dictionary of nulls (caller can handle null = unknown)
            var map = (tickers ?? Array.Empty<string>())
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => t.Trim().ToUpperInvariant())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToDictionary(t => t, _ => (double?)null, StringComparer.OrdinalIgnoreCase);

            return Task.FromResult<IDictionary<string, double?>>(map);
        }

        public async Task<double?> LastPrice(string ticker, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(ticker))
                return null;

            var dict = await GetPricesAsync(new[] { ticker }, ct).ConfigureAwait(false);
            return dict.TryGetValue(ticker.Trim().ToUpperInvariant(), out var price) ? price : null;
        }
    }
}
