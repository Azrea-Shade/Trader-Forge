using Services;
using System;
using System.IO;
using FluentAssertions;
using Xunit;

public class Phase3_ShellSmokeTests
{
    [Fact]
    public void Navigation_commands_switch_current_viewmodel()
    {
        // Arrange: temp DB
        var tmp = Path.Combine(Path.GetTempPath(), "azrea_phase3", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tmp);
        var dbPath = Path.Combine(tmp, "app.db");
        var db = new Infrastructure.SqliteDb(dbPath);

        // Repos + services required by VMs
        var favRepo = new Infrastructure.FavoritesRepository(db);
        var setRepo = new Infrastructure.SettingsRepository(db);
        var alRepo  = new Infrastructure.AlertsRepository(db);

        var favs  = new Services.FavoritesService(favRepo);
        var sets  = new Services.SettingsService(setRepo);
        var al    = new Services.AlertsService(alRepo);
        var paths = new Services.AppPathsService(db);

        var dash     = new Presentation.DashboardViewModel(paths);
        var facade   = new Services.WatchlistFacade(db);
        var priceFeed= new DummyPriceFeed();
        var watch    = new Presentation.WatchlistViewModel(facade, priceFeed);
        var favVM    = new Presentation.FavoritesViewModel(favs);
        var alVM     = new Presentation.AlertsViewModel(al);
        var setVM    = new Presentation.SettingsViewModel(sets);

        var shell = new Presentation.MainViewModel(dash, watch, favVM, alVM, setVM);

        // Act + Assert
        shell.CurrentViewModel.Should().Be(dash);
        shell.GoFavoritesCommand.Execute(null);
        shell.CurrentViewModel.Should().Be(favVM);
        shell.GoAlertsCommand.Execute(null);
        shell.CurrentViewModel.Should().Be(alVM);
        shell.GoSettingsCommand.Execute(null);
        shell.CurrentViewModel.Should().Be(setVM);
        shell.GoDashboardCommand.Execute(null);
        shell.CurrentViewModel.Should().Be(dash);
    }
}
