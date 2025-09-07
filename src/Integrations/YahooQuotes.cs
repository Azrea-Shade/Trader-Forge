using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Integrations
{
    public static class YahooQuotes
    {
        // Returns regular market price if available, else null.
        public static async Task<decimal?> GetLastPriceAsync(HttpClient http, string ticker, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(ticker)) return null;
            var sym = ticker.Trim().ToUpperInvariant();
            var url = $"https://query1.finance.yahoo.com/v7/finance/quote?symbols={Uri.EscapeDataString(sym)}";

            using var resp = await http.GetAsync(url, ct).ConfigureAwait(false);
            if (!resp.IsSuccessStatusCode) return null;

            await using var stream = await resp.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct).ConfigureAwait(false);

            if (!doc.RootElement.TryGetProperty("quoteResponse", out var qr)) return null;
            if (!qr.TryGetProperty("result", out var arr)) return null;
            if (arr.ValueKind != JsonValueKind.Array) return null;

            foreach (var item in arr.EnumerateArray())
            {
                if (!item.TryGetProperty("symbol", out var symEl)) continue;
                var s = symEl.GetString();
                if (!string.Equals(s, sym, StringComparison.OrdinalIgnoreCase)) continue;

                if (item.TryGetProperty("regularMarketPrice", out var pEl) &&
                    pEl.ValueKind == JsonValueKind.Number &&
                    pEl.TryGetDecimal(out var price) &&
                    price > 0)
                {
                    return price;
                }
            }
            return null;
        }
    }
}
