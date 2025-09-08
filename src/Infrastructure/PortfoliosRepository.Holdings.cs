using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Domain;

namespace Infrastructure
{
    public partial class PortfoliosRepository
    {
        // Map to Domain.Portfolio (Id:int, Name:string, Notes:string?)
        public IEnumerable<Portfolio> AllPortfolios()
            => WithConnection(conn => conn.Query<Portfolio>(
                @"SELECT CAST(id AS INT)   AS Id,
                         name              AS Name,
                         notes             AS Notes
                  FROM portfolios
                  ORDER BY id;"));

        public Portfolio GetPortfolio(int id)
            => WithConnection(conn => conn.QuerySingle<Portfolio>(
                @"SELECT CAST(id AS INT)   AS Id,
                         name              AS Name,
                         notes             AS Notes
                  FROM portfolios
                  WHERE id=@id;", new { id }));

        // Map to Domain.Holding (Id:int, PortfolioId:int, Symbol:string, Shares:double, Cost:double)
        public IEnumerable<Holding> Holdings(int portfolioId)
            => WithConnection(conn => conn.Query<Holding>(
                @"SELECT CAST(id AS INT)           AS Id,
                         CAST(portfolio_id AS INT) AS PortfolioId,
                         symbol                    AS Symbol,
                         CAST(shares AS REAL)      AS Shares,
                         CAST(cost   AS REAL)      AS Cost
                  FROM holdings
                  WHERE portfolio_id=@portfolioId
                  ORDER BY id;", new { portfolioId }));

        // NOTE: Services call with (int,double,double). Accept doubles and store to REAL.
        public void AddHolding(int portfolioId, string symbol, double shares, double cost)
            => WithConnection(conn => conn.Execute(
                @"INSERT INTO holdings (portfolio_id, symbol, shares, cost)
                  VALUES (@portfolioId, @symbol, @shares, @cost);",
                new { portfolioId, symbol, shares, cost }));

        public void UpdateHolding(int holdingId, double shares, double cost)
            => WithConnection(conn => conn.Execute(
                @"UPDATE holdings SET shares=@shares, cost=@cost WHERE id=@holdingId;",
                new { holdingId, shares, cost }));

        public void RemoveHolding(int holdingId)
            => WithConnection(conn => conn.Execute(
                "DELETE FROM holdings WHERE id=@holdingId;", new { holdingId }));

        // Keep summary dynamic — Services typically use 'var'
        public object BuildSummary(int portfolioId)
            => WithConnection(conn =>
            {
                var rows = conn.Query<(string symbol, double shares, double costPerUnit)>(
                    @"SELECT symbol,
                             CAST(shares AS REAL) AS shares,
                             CAST(cost   AS REAL) AS costPerUnit
                      FROM holdings WHERE portfolio_id=@portfolioId;",
                    new { portfolioId }).ToList();

                var values  = rows.Select(r => (r.symbol, value: r.shares * r.costPerUnit)).ToList();
                var total   = values.Sum(v => v.value);
                var weights = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

                if (total > 0)
                    foreach (var (symbol, value) in values)
                        weights[symbol] = Math.Round(value / total, 6);
                else
                    foreach (var (symbol, _) in values)
                        weights[symbol] = 0.0;

                return new { totalCost = total, weights };
            });
    }
}
