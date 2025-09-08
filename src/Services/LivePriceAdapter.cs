using Services;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    /// <summary>
    /// Adapts LivePriceRouter to the existing IPriceFeed contract used by the project.
    /// </summary>
    public sealed class LivePriceAdapter : IPriceFeed
    {
        private readonly LivePriceRouter _router = new LivePriceRouter();

        public double LastPrice(string ticker)
        {
            // Convert router's decimal to double for interface compatibility
            var p = _router.LastPrice(ticker);
            return (double)p;
        }

        public Task<IDictionary<string, double>> GetPricesAsync(IEnumerable<string> tickers, CancellationToken ct = default)
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var t in tickers ?? Array.Empty<string>())
            {
                if (!string.IsNullOrWhiteSpace(t)) set.Add(t.Trim());
            }

            var dict = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
            foreach (var s in set)
            {
                if (ct.IsCancellationRequested) break;
                dict[s] = LastPrice(s);
            }

            return Task.FromResult<IDictionary<string, double>>(dict);
        }
    }
}
