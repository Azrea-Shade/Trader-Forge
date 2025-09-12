using Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Services.Engines;
using Xunit;

public class Phase4_AlertEvalWithWatchlist
{
    [Fact]
    public async Task Evaluate_alerts_against_dummy_prices()
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
        var prices = await feed.GetPricesAsync(tickers);

        var p1 = prices["AAPL"];
        var p2 = prices["MSFT"];

        // IMPORTANT: AlertEngine uses inclusive comparisons:
        //  - TriggeredAbove when price >= Above
        //  - TriggeredBelow when price <= Below
        // For "no trigger", set Above ABOVE the price and Below BELOW the price.
        facade.SetThresholds(id1, p1 + 1, p1 - 1); // no trigger expected

        // For a positive "above" trigger, keep Above below/equal to price, Below null.
        facade.SetThresholds(id2, p2 - 10, null);  // expect TriggeredAbove == true

        var rules = new[]
        {
            new Infrastructure.AlertRow(id1, "AAPL", p1 + 1, p1 - 1, true),
            new Infrastructure.AlertRow(id2, "MSFT", p2 - 10, null, true),
        };

        // Act
        var evals = AlertEngine.Evaluate(rules, prices).ToList();

        // Assert
        evals.Should().Contain(e => e.Id == id1 && !e.TriggeredAbove && !e.TriggeredBelow);
        evals.Should().Contain(e => e.Id == id2 &&  e.TriggeredAbove && !e.TriggeredBelow);
    }
}
