using System;
using System.IO;
using FluentAssertions;
using Xunit;

public class Phase1Tests
{
    [Fact]
    public void Database_initializes_with_all_tables_and_settings_seed()
    {
        var tmp = Path.Combine(Path.GetTempPath(), "azrea_phase1", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tmp);
        var dbFile = Path.Combine(tmp, "phase1.db");

        var db = new Infrastructure.SqliteDb(dbFile);

        // Settings seed present
        var settings = new Infrastructure.SettingsRepository(db);
        settings.Get("Brief.GenerateAt").Should().Be("07:30");
        settings.Get("Brief.NotifyAt").Should().Be("08:00");
        settings.Get("Autostart").Should().Be("On");

        // Favorites & Alerts usable
        var favs = new Infrastructure.FavoritesRepository(db);
        favs.Count().Should().Be(0);
        favs.Add("MSFT");
        favs.Count().Should().Be(1);

        var alerts = new Infrastructure.AlertsRepository(db);
        alerts.Count().Should().Be(0);
        var id = alerts.Add("MSFT", above: 500.0);
        id.Should().BeGreaterThan(0);
        alerts.Count().Should().Be(1);
    }
}
