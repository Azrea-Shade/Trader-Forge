using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Feeds
{
    public interface IPriceFeed
    {
        Task<IDictionary<string, double>> GetPricesAsync(IEnumerable<string> tickers, CancellationToken ct = default);
    }
}
