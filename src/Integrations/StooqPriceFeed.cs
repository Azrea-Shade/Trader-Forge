using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Services;

namespace Integrations
{
    /// <summary>
    /// Very simple fallback using stooq CSV (often daily). Best-effort last close.
    /// </summary>
    public sealed class StooqPriceFeed : IPriceFeed
    {
        private readonly HttpClient _http;
        public StooqPriceFeed(HttpClient http) => _http = http;

        public async Task<IDictionary<string, decimal>> GetPricesAsync(IEnumerable<string> tickers, CancellationToken ct = default)
        {
            var list = tickers.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim().ToLowerInvariant()).Distinct().ToList();
            if (list.Count == 0) return new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

            // stooq wants lowercase symbols separated by commas
            var url = $"https://stooq.com/q/l/?s={string.Join(",", list)}&f=sd2t2ohlcv&h&e=csv";
            using var resp = await _http.GetAsync(url, ct).ConfigureAwait(false);
            resp.EnsureSuccessStatusCode();
            var text = await resp.Content.ReadAsStringAsync(ct).ConfigureAwait(false);

            var result = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            var lines = text.Split('\n');
            // Skip header; parse Close column (index 6)
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;
                var parts = line.Split(',');
                if (parts.Length < 7) continue;
                var sym = parts[0].Trim().ToUpperInvariant();
                if (decimal.TryParse(parts[6], NumberStyles.Any, CultureInfo.InvariantCulture, out var close) && close > 0)
                {
                    result[sym] = close;
                }
            }
            return result;
        }
    }
}
