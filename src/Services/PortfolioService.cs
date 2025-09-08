using System.Collections.Generic;
using Infrastructure;
using Domain;

namespace Services
{
    public class PortfolioService
    {
        private readonly PortfoliosRepository _repo;

        public PortfolioService(PortfoliosRepository repo)
        {
            _repo = repo;
        }

        // Create a portfolio and return its id
        public long CreatePortfolio(string name, string? notes)
        {
            var id = _repo.CreatePortfolio(name, notes);
            return id;
        }

        // List all portfolios
        public IEnumerable<Portfolio> All()
            => _repo.AllPortfolios();

        // Holdings for a portfolio
        public IEnumerable<Holding> Holdings(long portfolioId)
            => _repo.Holdings(portfolioId);

        // Add / update / remove holdings (repo methods return void)
        public void AddHolding(long portfolioId, string symbol, double shares, double cost)
            => _repo.AddHolding(portfolioId, symbol, shares, cost);

        public void UpdateHolding(long holdingId, double shares, double cost)
            => _repo.UpdateHolding(holdingId, shares, cost);

        public void RemoveHolding(long holdingId)
            => _repo.RemoveHolding(holdingId);

        // Build allocation/summary using repo's single-arg overload
        public object Summary(long portfolioId)
            => _repo.BuildSummary(portfolioId);

        // Convenience helpers
        public Portfolio Get(long portfolioId)
            => _repo.GetPortfolio(portfolioId);

        public void Rename(long portfolioId, string name, string? notes)
            => _repo.UpdatePortfolio(portfolioId, name, notes);

        public void Delete(long portfolioId)
            => _repo.DeletePortfolio(portfolioId);
    }
}
