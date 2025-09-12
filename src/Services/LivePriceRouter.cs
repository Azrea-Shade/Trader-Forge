using Services;
using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Integrations;

namespace Services
{
    /// <summary>
    /// Live price fetcher with 15s cache and Yahoo→Stooq fallback.
    /// </summary>
    public sealed class LivePriceRouter
    {
        private static readonly HttpClient _http = new HttpClient();
        private readonly TimeSpan _ttl = TimeSpan.FromSeconds(15);

        // cache: ticker -> (price, timestamp)
        private readonly ConcurrentDictionary<string,(decimal price, DateTime ts)> _cache =
            new(StringComparer.OrdinalIgnoreCase);

        public decimal LastPrice(string ticker)
        {
            if (string.IsNullOrWhiteSpace(ticker)) return 0m;
            var key = ticker.Trim().ToUpperInvariant();
            var now = DateTime.UtcNow;

            if (_cache.TryGetValue(key, out var entry) && (now - entry.ts) < _ttl)
                return entry.price;

            var price = FetchAsync(key, CancellationToken.None).GetAwaiter().GetResult() ?? 0m;
            _cache[key] = (price, now);
            return price;
        }

        private async Task<decimal?> FetchAsync(string ticker, CancellationToken ct)
        {
            try
            {
                var y = await YahooQuotes.GetLastPriceAsync(_http, ticker, ct).ConfigureAwait(false);
                if (y.HasValue && y.Value > 0) return y.Value;
            }
            catch { /* ignore and fallback */ }

            try
            {
                var s = await StooqQuotes.GetLastCloseAsync(_http, ticker, ct).ConfigureAwait(false);
                if (s.HasValue && s.Value > 0) return s.Value;
            }
            catch { /* give up */ }

            return null;
        }
    }
}
