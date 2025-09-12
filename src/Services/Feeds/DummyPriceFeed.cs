using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Feeds
{
    public class DummyPriceFeed : IPriceFeed
    {
        public Task<IDictionary<string, double?>> GetPricesAsync(IEnumerable<string> tickers, CancellationToken cancellationToken)
        {
            var dict = tickers.Distinct().ToDictionary(t => t, t => (double?)null);
            return Task.FromResult((IDictionary<string,double?>)dict);
        }

        public Task<double?> LastPrice(string ticker, CancellationToken cancellationToken)
            => Task.FromResult<double?>(null);
    }
}
