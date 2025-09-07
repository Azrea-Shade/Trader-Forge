using System;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Integrations
{
    public static class StooqQuotes
    {
        // Returns last close if available, else null.
        public static async Task<decimal?> GetLastCloseAsync(HttpClient http, string ticker, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(ticker)) return null;
            var sym = ticker.Trim().ToLowerInvariant();
            var url = $"https://stooq.com/q/l/?s={Uri.EscapeDataString(sym)}&f=sd2t2ohlcv&h&e=csv";

            using var resp = await http.GetAsync(url, ct).ConfigureAwait(false);
            if (!resp.IsSuccessStatusCode) return null;

            var text = await resp.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
            var lines = text.Split('\n');
            if (lines.Length < 2) return null; // header + row
            var row = lines[1].Trim();
            if (string.IsNullOrEmpty(row)) return null;
            var parts = row.Split(',');
            if (parts.Length < 7) return null;
            if (decimal.TryParse(parts[6], NumberStyles.Any, CultureInfo.InvariantCulture, out var close) && close > 0)
                return close;
            return null;
        }
    }
}
