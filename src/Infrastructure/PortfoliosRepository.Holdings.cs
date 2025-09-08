using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;

namespace Infrastructure
{
    public partial class PortfoliosRepository
    {
        // List all portfolios (id, name, notes)
        public IEnumerable<dynamic> AllPortfolios()
            => WithConnection(conn => conn.Query("SELECT id, name, notes FROM portfolios ORDER BY id;"));

        // Get a single portfolio
        public dynamic GetPortfolio(int id)
            => WithConnection(conn => conn.QuerySingle("SELECT id, name, notes FROM portfolios WHERE id=@id;", new { id }));

        // Holdings for a portfolio
        public IEnumerable<dynamic> Holdings(int portfolioId)
            => WithConnection(conn => conn.Query(
                @"SELECT id,
                         portfolio_id AS portfolioId,
                         symbol,
                         CAST(shares AS REAL) AS shares,
                         CAST(cost   AS REAL) AS cost
                  FROM holdings
                  WHERE portfolio_id=@portfolioId
                  ORDER BY id;", new { portfolioId }));

        public void AddHolding(int portfolioId, string symbol, decimal shares, decimal cost)
            => WithConnection(conn => conn.Execute(
                @"INSERT INTO holdings (portfolio_id, symbol, shares, cost)
                  VALUES (@portfolioId, @symbol, @shares, @cost);",
                new { portfolioId, symbol, shares, cost }));

        public void UpdateHolding(int holdingId, decimal shares, decimal cost)
            => WithConnection(conn => conn.Execute(
                @"UPDATE holdings SET shares=@shares, cost=@cost WHERE id=@holdingId;",
                new { holdingId, shares, cost }));

        public void RemoveHolding(int holdingId)
            => WithConnection(conn => conn.Execute(
                "DELETE FROM holdings WHERE id=@holdingId;", new { holdingId }));

        // Simple summary: total cost and weights by cost
        public dynamic BuildSummary(int portfolioId)
            => WithConnection(conn =>
            {
                var rows = conn.Query<(string symbol, decimal shares, decimal costPerUnit)>(
                    @"SELECT symbol,
                             CAST(shares AS REAL) AS shares,
                             CAST(cost   AS REAL) AS costPerUnit
                      FROM holdings WHERE portfolio_id=@portfolioId;",
                    new { portfolioId }).ToList();

                var values = rows.Select(r => (r.symbol, value: r.shares * r.costPerUnit)).ToList();
                var total  = values.Sum(v => v.value);
                var weights = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

                if (total > 0)
                {
                    foreach (var (symbol, value) in values)
                        weights[symbol] = weights.TryGetValue(symbol, out var w) ? w + decimal.Round(value / total, 6) : decimal.Round(value / total, 6);
                }
                else
                {
                    foreach (var (symbol, _) in values)
                        weights[symbol] = 0m;
                }

                return new { totalCost = total, weights };
            });
    }
}
