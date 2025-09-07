using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Feeds
{
    /// <summary>
    /// Deterministic pseudo-prices based on ticker text + current UTC date.
    /// Stable across runs on the same day to keep tests/CLI output predictable.
    /// </summary>
    public class DummyPriceFeed : IPriceFeed
    {
        public Task<IDictionary<string, double>> GetPricesAsync(IEnumerable<string> tickers, CancellationToken ct = default)
        {
            var dict = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
            var seedDate = DateTime.UtcNow.Date;

            foreach (var t in tickers.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                ct.ThrowIfCancellationRequested();
                dict[t] = PriceFor(t, seedDate);
            }
            return Task.FromResult<IDictionary<string, double>>(dict);
        }

        private static double PriceFor(string ticker, DateTime date)
        {
            // Hash: ticker + yyyyMMdd -> 64-bit integer -> normalized
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes($"{ticker.ToUpperInvariant()}:{date:yyyyMMdd}"));
            // Take first 8 bytes as ulong
            ulong u = BitConverter.ToUInt64(bytes, 0);
            var frac = (u / (double)ulong.MaxValue); // [0,1)
            // Map to a plausible range per ticker
            var baseMin = 10.0;
            var baseMax = 500.0;
            var price = baseMin + frac * (baseMax - baseMin);
            // Round to cents
            return Math.Round(price, 2);
        }
    }
}
