public class Phase4_WatchlistRepoTests
{
    [Fact]
    public void Add_update_remove_roundtrip()
    {
        // Arrange: isolated temp DB
        var tmp = Path.Combine(Path.GetTempPath(), "azrea_phase4", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tmp);
        var dbPath = Path.Combine(tmp, "app.db");
        var db = new Infrastructure.SqliteDb(dbPath);

        var facade = new Services.WatchlistFacade(db);

        // Add
        var id = facade.Add("MSFT");
        id.Should().BeGreaterThan(0);

        // Update thresholds
        facade.SetThresholds(id, 400, 320);

        // Verify
        var all = facade.All();
        all.Should().Contain(x => x.Id == id && x.Ticker == "MSFT" && x.Above == 400 && x.Below == 320);

        // Remove
        facade.Remove(id);
        facade.All().Should().NotContain(x => x.Id == id);
    }
}
