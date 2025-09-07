using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Services;
using Integrations;

namespace Services
{
    /// <summary>
    /// Orchestrates live fetching with 15s cache TTL and fallbacks (Yahoo → Stooq).
    /// </summary>
    public sealed class LivePriceRouter : IPriceFeed
    {
        private readonly IPriceFeed _primary;   // Yahoo
        private readonly IPriceFeed _fallback;  // Stooq
        private readonly TimeSpan _ttl = TimeSpan.FromSeconds(15);

        private readonly Dictionary<string,(decimal price, DateTime ts)> _cache =
            new(StringComparer.OrdinalIgnoreCase);

        public LivePriceRouter(HttpClient http)
        {
            _primary = new YahooPriceFeed(http);
            _fallback = new StooqPriceFeed(http);
        }

        public async Task<IDictionary<string, decimal>> GetPricesAsync(IEnumerable<string> tickers, CancellationToken ct = default)
        {
            var req = tickers.Where(x => !string.IsNullOrWhiteSpace(x))
                             .Select(x => x.Trim().ToUpperInvariant())
                             .Distinct()
                             .ToList();

            var now = DateTime.UtcNow;
            var result = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            var missing = new List<string>();

            // 1) serve fresh cache
            foreach (var t in req)
            {
                if (_cache.TryGetValue(t, out var entry) && (now - entry.ts) < _ttl)
                {
                    result[t] = entry.price;
                }
                else
                {
                    missing.Add(t);
                }
            }

            if (missing.Count == 0) return result;

            // 2) fetch from primary (Yahoo)
            var http = new HttpClient();
            IDictionary<string, decimal> got = new Dictionary<string, decimal>();
            try
            {
                got = await _primary.GetPricesAsync(missing, ct).ConfigureAwait(false);
            }
            catch
            {
                // swallow and fallback
            }

            // 3) fill any remaining via fallback (Stooq)
            var still = missing.Where(t => !got.ContainsKey(t)).ToList();
            if (still.Count > 0)
            {
                try
                {
                    var fb = await _fallback.GetPricesAsync(still, ct).ConfigureAwait(false);
                    foreach (var kv in fb) got[kv.Key] = kv.Value;
                }
                catch { /* give up */ }
            }

            // 4) update cache + result
            foreach (var kv in got)
            {
                _cache[kv.Key] = (kv.Value, now);
                result[kv.Key] = kv.Value;
            }

            return result;
        }
    }
}
