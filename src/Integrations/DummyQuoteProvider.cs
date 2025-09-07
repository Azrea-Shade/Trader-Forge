using System;
using Domain;

namespace Integrations
{
    public class DummyQuoteProvider : IQuoteProvider
    {
        private readonly Random _rng = new();
        public decimal? TryGetPrice(string ticker) => Math.Round((decimal)(_rng.NextDouble()*100 + 5), 2);
    }
}
