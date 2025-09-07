using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Services;

namespace Integrations
{
    /// <summary>
    /// Fetches quotes via Yahoo-finance "quote" endpoint (no key). Not official; subject to change.
    /// </summary>
    public sealed class YahooPriceFeed : IPriceFeed
    {
        private readonly HttpClient _http;
        public YahooPriceFeed(HttpClient http) => _http = http;

        public async Task<IDictionary<string, decimal>> GetPricesAsync(IEnumerable<string> tickers, CancellationToken ct = default)
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var t in tickers) if (!string.IsNullOrWhiteSpace(t)) set.Add(t.Trim());
            if (set.Count == 0) return new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

            var symbols = string.Join(",", set);
            var url = $"https://query1.finance.yahoo.com/v7/finance/quote?symbols={Uri.EscapeDataString(symbols)}";

            using var resp = await _http.GetAsync(url, ct).ConfigureAwait(false);
            resp.EnsureSuccessStatusCode();
            await using var stream = await resp.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct).ConfigureAwait(false);

            var result = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            if (doc.RootElement.TryGetProperty("quoteResponse", out var qr) &&
                qr.TryGetProperty("result", out var arr) &&
                arr.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in arr.EnumerateArray())
                {
                    if (!item.TryGetProperty("symbol", out var symEl)) continue;
                    var sym = symEl.GetString();
                    decimal price = 0;
                    if (item.TryGetProperty("regularMarketPrice", out var pEl))
                    {
                        // Some symbols may return null/NaN; handle cautiously.
                        if (pEl.ValueKind == JsonValueKind.Number && pEl.TryGetDecimal(out var p))
                            price = p;
                    }
                    if (!string.IsNullOrWhiteSpace(sym) && price > 0)
                        result[sym] = price;
                }
            }
            return result;
        }
    }
}
