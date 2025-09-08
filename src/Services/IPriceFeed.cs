using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public interface IPriceFeed
    {
        Task<double?> LastPrice(string ticker, CancellationToken ct = default);
        Task<IDictionary<string, double?>> GetPricesAsync(IEnumerable<string> tickers, CancellationToken ct = default);
    }
}
