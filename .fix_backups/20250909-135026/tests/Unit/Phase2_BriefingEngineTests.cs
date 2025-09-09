public class Phase2_BriefingEngineTests
{
    [Fact]
    public void Brief_contains_header_and_ticker_lines()
    {
        var tickers = new[] { "MSFT", "AAPL" };
        var prices = new Dictionary<string, double> { ["MSFT"] = 350.12, ["AAPL"] = 189.55 };

        var lines = BriefingEngine.BuildBrief(tickers, prices, new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc));

        lines.Count.Should().BeGreaterThan(2);
        lines[0].Should().Contain("Good morning");
        lines.Should().Contain(l => l.Contains("MSFT"));
        lines.Should().Contain(l => l.Contains("AAPL"));
    }
}
