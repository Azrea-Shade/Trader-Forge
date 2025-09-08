using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    /// <summary>
    /// Simple implementation of IPriceFeed used as a safe default.
    /// Returns null prices (caller can handle null = unknown). This unblocks builds.
    /// Swap for a real feed in PriceFeedRegistration when ready.
    /// </summary>
    public sealed class DummyPriceFeed : IPriceFeed
    {
        private readonly double? _fixedPrice;

        public DummyPriceFeed(double? fixedPrice = null)
        {
            _fixedPrice = fixedPrice;
        }

        public Task<IDictionary<string, double?>> GetPricesAsync(IEnumerable<string> tickers, CancellationToken ct = default)
        {
            var keys = (tickers ?? Array.Empty<string>())
                       .Where(t => !string.IsNullOrWhiteSpace(t))
                       .Select(t => t.Trim().ToUpperInvariant())
                       .Distinct(StringComparer.OrdinalIgnoreCase);

            var map = new Dictionary<string, double?>(StringComparer.OrdinalIgnoreCase);
            foreach (var k in keys)
            {
                map[k] = _fixedPrice; // null by default
            }
            return Task.FromResult<IDictionary<string, double?>>(map);
        }

        public Task<double?> LastPrice(string ticker, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(ticker)) return Task.FromResult<double?>(null);
            return Task.FromResult(_fixedPrice);
        }
    }
}
