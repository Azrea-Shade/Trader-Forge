using System.Collections.Generic;
using FluentAssertions;
using Services.Engines;
using Xunit;

public class Phase2_AlertEngineTests
{
    [Fact]
    public void Triggers_above_and_below_thresholds()
    {
        var prices = new Dictionary<string, double> { ["MSFT"] = 350.0, ["AAPL"] = 180.0 };
        var rules = new[]
        {
            new Infrastructure.AlertRow(1, "MSFT", 340.0, null, true),
            new Infrastructure.AlertRow(2, "MSFT", null, 360.0, true),
            new Infrastructure.AlertRow(3, "AAPL", 200.0, 150.0, true),
        };

        var results = AlertEngine.Evaluate(rules, prices);

        results.Should().Contain(r => r.Id == 1 && r.TriggeredAbove && !r.TriggeredBelow);
        results.Should().Contain(r => r.Id == 2 && !r.TriggeredAbove && !r.TriggeredBelow);
        results.Should().Contain(r => r.Id == 3 && !r.TriggeredAbove && !r.TriggeredBelow);
    }
}
