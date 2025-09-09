using Domain.Entities;
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
        public IEnumerable<Portfolio> AllPortfolios()
            => WithConnection(conn => conn.Query<Portfolio>(
                @"SELECT id AS Id, name AS Name, notes AS Notes
                  FROM portfolios ORDER BY id;"));

        public Portfolio GetPortfolio(long id)
            => WithConnection(conn => conn.QuerySingle<Portfolio>(
                @"SELECT id AS Id, name AS Name, notes AS Notes
                  FROM portfolios WHERE id=@id;", new { id = checked((int)id) }));

        public IEnumerable<Holding> Holdings(long portfolioId)
            => WithConnection(conn => conn.Query<Holding>(
                @"SELECT id AS Id,
                         portfolio_id AS PortfolioId,
                         symbol AS Symbol,
                         shares AS Shares,
                         cost   AS Cost
                  FROM holdings
                  WHERE portfolio_id=@pid
                  ORDER BY id;", new { pid = checked((int)portfolioId) }));

        public void AddHolding(long portfolioId, string symbol, double shares, double cost)
            => WithConnection(conn => conn.Execute(
                @"INSERT INTO holdings (portfolio_id, symbol, shares, cost)
                  VALUES (@pid, @symbol, @shares, @cost);",
                new { pid = checked((int)portfolioId), symbol, shares, cost }));

        public void UpdateHolding(long holdingId, double shares, double cost)
            => WithConnection(conn => conn.Execute(
                @"UPDATE holdings SET shares=@shares, cost=@cost WHERE id=@hid;",
                new { hid = checked((int)holdingId), shares, cost }));

        public void RemoveHolding(long holdingId)
            => WithConnection(conn => conn.Execute(
                "DELETE FROM holdings WHERE id=@hid;", new { hid = checked((int)holdingId) }));

        public object BuildSummary(long portfolioId)
            => WithConnection(conn =>
            {
                var rows = conn.Query<(string symbol, double shares, double costPerUnit)>(
                    @"SELECT symbol,
                             shares       AS shares,
                             cost         AS costPerUnit
                      FROM holdings WHERE portfolio_id=@pid;",
                    new { pid = checked((int)portfolioId) }).ToList();

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
