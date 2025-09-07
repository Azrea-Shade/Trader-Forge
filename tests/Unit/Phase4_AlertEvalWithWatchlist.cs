using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Services.Engines;
using Services.Feeds;
using Xunit;

public class Phase4_AlertEvalWithWatchlist
{
    [Fact]
    public void Evaluate_alerts_against_dummy_prices()
    {
        // Arrange: temp DB
        var tmp = Path.Combine(Path.GetTempPath(), "azrea_phase4", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tmp);
        var dbPath = Path.Combine(tmp, "app.db");
        var db = new Infrastructure.SqliteDb(dbPath);
        var facade = new Services.WatchlistFacade(db);

        var id1 = facade.Add("AAPL");
        var id2 = facade.Add("MSFT");

        var feed = new DummyPriceFeed();
        var tickers = new[] { "AAPL", "MSFT" };
        var prices = feed.GetPricesAsync(tickers).GetAwaiter().GetResult();

        // Choose thresholds around today's deterministic price
        var p1 = prices["AAPL"];
        var p2 = prices["MSFT"];
        facade.SetThresholds(id1, p1 - 1, p1 + 1); // price is between: no trigger
        facade.SetThresholds(id2, p2 - 10, null);  // above triggers (price >= above)

        var rules = new[]
        {
            new Infrastructure.AlertRow(id1, "AAPL", p1 - 1, p1 + 1, true),
            new Infrastructure.AlertRow(id2, "MSFT", p2 - 10, null, true),
        };

        // Act
        var evals = AlertEngine.Evaluate(rules, prices).ToList();

        // Assert
        evals.Should().Contain(e => e.Id == id1 && !e.TriggeredAbove && !e.TriggeredBelow);
        evals.Should().Contain(e => e.Id == id2 &&  e.TriggeredAbove && !e.TriggeredBelow);
    }
}
