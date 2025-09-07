using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using Infrastructure;

namespace Services
{
    public interface IPriceFeed
    {
        // Returns latest price; implementations: dummy, api-backed, etc.
        double LastPrice(string ticker);
    }

    public class DummyPriceFeed : IPriceFeed
    {
        private readonly Dictionary<string,double> _map = new(StringComparer.OrdinalIgnoreCase)
        {
            ["AAPL"] = 315.19,
            ["MSFT"] = 169.24,
            ["SPY"]  = 500.00
        };
        public double LastPrice(string ticker) => _map.TryGetValue(ticker, out var p) ? p : 100.00;
    }

    public class PortfolioService
    {
        private readonly PortfoliosRepository _repo;
        private readonly IPriceFeed _prices;

        public PortfolioService(PortfoliosRepository repo, IPriceFeed prices)
        {
            _repo = repo;
            _prices = prices;
        }

        public long CreatePortfolio(string name, string? notes = null) => _repo.CreatePortfolio(name, notes);
        public IEnumerable<Portfolio> AllPortfolios() => _repo.AllPortfolios();
        public IEnumerable<Holding> Holdings(long portfolioId) => _repo.Holdings(portfolioId);
        public long AddHolding(long portfolioId, string ticker, double shares, double costBasis) =>
            _repo.AddHolding(portfolioId, ticker, shares, costBasis);
        public void UpdateHolding(long holdingId, double shares, double costBasis) =>
            _repo.UpdateHolding(holdingId, shares, costBasis);
        public void RemoveHolding(long holdingId) => _repo.RemoveHolding(holdingId);

        public PortfolioSummary Summary(long portfolioId)
        {
            var p = _repo.GetPortfolio(portfolioId) ?? throw new InvalidOperationException("Portfolio not found");
            var hs = _repo.Holdings(portfolioId);
            double PriceFor(string t) => _prices.LastPrice(t);
            return _repo.BuildSummary(p, hs, PriceFor);
        }
    }
}
