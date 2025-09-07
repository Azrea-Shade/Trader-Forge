using System;

namespace Services
{
    /// <summary>
    /// Adapts LivePriceRouter to the existing IPriceFeed (LastPrice) contract.
    /// </summary>
    public sealed class LivePriceAdapter : IPriceFeed
    {
        private readonly LivePriceRouter _router = new LivePriceRouter();
        public decimal LastPrice(string ticker) => _router.LastPrice(ticker);
    }
}
