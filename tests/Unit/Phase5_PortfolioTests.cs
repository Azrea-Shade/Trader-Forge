using System;
using System.IO;
using Domain;
using Infrastructure;
using Services;
using Xunit;
using FluentAssertions;

public class Phase5_PortfolioTests
{
    private static string TempDb()
    {
        var p = Path.Combine(Path.GetTempPath(), $"azrea_portfolio_{Guid.NewGuid():N}.db");
        if (File.Exists(p)) File.Delete(p);
        return p;
    }

    [Fact]
    public void Portfolio_crud_and_allocation_computes_weights()
    {
        var dbfile = TempDb();
        var db = new SqliteDb(dbfile);
        var repo = new PortfoliosRepository(db);
        var prices = new DummyPriceFeed();
        var svc = new PortfolioService(repo, prices);

        var pid = svc.CreatePortfolio("TestPort");
        svc.AddHolding(pid, "AAPL", 10, 2500); // price 315.19 -> 3151.90 MV
        svc.AddHolding(pid, "MSFT", 5, 700);   // price 169.24 -> 846.20 MV

        var s = svc.Summary(pid);
        s.TotalMarketValue.Should().BeApproximately(3998.10, 0.5);
        s.TotalCost.Should().Be(3200);
        s.GainLoss.Should().BeApproximately(798.10, 1.0);
        s.Allocation.Should().HaveCount(2);
        var aapl = s.Allocation.Find(a => a.Ticker == "AAPL")!;
        var msft = s.Allocation.Find(a => a.Ticker == "MSFT")!;
        (aapl.WeightPct + msft.WeightPct).Should().BeApproximately(100.0, 0.1);
        aapl.MarketValue.Should().BeGreaterThan(msft.MarketValue);
    }
}
