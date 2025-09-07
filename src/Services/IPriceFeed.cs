using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    /// <summary>
    /// Simple multi-symbol price API returning last trade (or close) per ticker.
    /// </summary>
    public interface IPriceFeed
    {
        Task<IDictionary<string, decimal>> GetPricesAsync(IEnumerable<string> tickers, CancellationToken ct = default);
    }
}
