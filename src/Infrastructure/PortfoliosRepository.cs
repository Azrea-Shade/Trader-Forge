using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Microsoft.Data.Sqlite;
using Domain;

namespace Infrastructure
{
    public class PortfoliosRepository
    {
        private readonly SqliteDb _db;
        public PortfoliosRepository(SqliteDb db) => _db = db;

        private SqliteConnection Open()
        {
            var cn = new SqliteConnection($"Data Source={_db.DbPath}");
            cn.Open();
            return cn;
        }

        public long CreatePortfolio(string name, string? notes = null)
        {
            using var cn = Open();
            return cn.ExecuteScalar<long>(
                "INSERT INTO portfolios(name,notes) VALUES(@name,@notes); SELECT last_insert_rowid();",
                new { name, notes });
        }

        public IEnumerable<Portfolio> AllPortfolios()
        {
            using var cn = Open();
            return cn.Query<Portfolio>("SELECT id, name, notes FROM portfolios ORDER BY name;");
        }

        public Portfolio? GetPortfolio(long id)
        {
            using var cn = Open();
            return cn.QuerySingleOrDefault<Portfolio>("SELECT id,name,notes FROM portfolios WHERE id=@id;", new { id });
        }

        public void RenamePortfolio(long id, string newName, string? notes = null)
        {
            using var cn = Open();
            cn.Execute("UPDATE portfolios SET name=@newName, notes=@notes WHERE id=@id;", new { id, newName, notes });
        }

        public void DeletePortfolio(long id)
        {
            using var cn = Open();
            cn.Execute("DELETE FROM portfolios WHERE id=@id;", new { id });
        }

        public long AddHolding(long portfolioId, string ticker, double shares, double costBasis)
        {
            using var cn = Open();
            return cn.ExecuteScalar<long>(
                @"INSERT INTO holdings(portfolio_id,ticker,shares,cost_basis)
                  VALUES(@portfolioId,@ticker,@shares,@costBasis);
                  SELECT last_insert_rowid();",
                new { portfolioId, ticker, shares, costBasis });
        }

        public void UpdateHolding(long holdingId, double shares, double costBasis)
        {
            using var cn = Open();
            cn.Execute("UPDATE holdings SET shares=@shares, cost_basis=@costBasis WHERE id=@holdingId;",
                new { holdingId, shares, costBasis });
        }

        public void RemoveHolding(long holdingId)
        {
            using var cn = Open();
            cn.Execute("DELETE FROM holdings WHERE id=@holdingId;", new { holdingId });
        }

        public IEnumerable<Holding> Holdings(long portfolioId)
        {
            using var cn = Open();
            return cn.Query<Holding>(
                "SELECT id, portfolio_id AS PortfolioId, ticker, shares, cost_basis AS CostBasis FROM holdings WHERE portfolio_id=@portfolioId ORDER BY ticker;",
                new { portfolioId });
        }

        // Allocation summary requires price lookup; caller provides price map
        public PortfolioSummary BuildSummary(Portfolio p, IEnumerable<Holding> holdings, Func<string,double> priceFor)
        {
            var list = new List<AllocationRow>();
            double totalValue = 0, totalCost = 0;
            foreach (var h in holdings)
            {
                var price = priceFor(h.Ticker);
                var value = price * h.Shares;
                totalValue += value;
                totalCost  += h.CostBasis;
                list.Add(new AllocationRow { Ticker = h.Ticker, MarketValue = value });
            }
            // weights
            foreach (var a in list)
            {
                a.WeightPct = totalValue > 0 ? (a.MarketValue / totalValue) * 100.0 : 0;
            }
            var gain = totalValue - totalCost;
            var glpct = totalCost > 0 ? (gain / totalCost) * 100.0 : 0;

            return new PortfolioSummary
            {
                PortfolioId = p.Id,
                Name = p.Name,
                TotalCost = Math.Round(totalCost, 2),
                TotalMarketValue = Math.Round(totalValue, 2),
                GainLoss = Math.Round(gain, 2),
                GainLossPct = Math.Round(glpct, 2),
                Allocation = list.OrderByDescending(x => x.WeightPct).ToList()
            };
        }
    }
}
