using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Presentation
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty] private string title = "Azrea — Companion";
        [ObservableProperty] private object? currentViewModel;

        // Keep singletons of page VMs for simple navigation
        private readonly DashboardViewModel _dashboard;
        private readonly WatchlistViewModel _watchlist;
        private readonly FavoritesViewModel _favorites;
        private readonly AlertsViewModel _alerts;
        private readonly SettingsViewModel _settings;

        public MainViewModel(
            DashboardViewModel dashboard,
            WatchlistViewModel watchlist,
            FavoritesViewModel favorites,
            AlertsViewModel alerts,
            SettingsViewModel settings)
        {
            _dashboard = dashboard;
            _watchlist = watchlist;
            _favorites = favorites;
            _alerts = alerts;
            _settings = settings;

            CurrentViewModel = _dashboard; // default
        }

        [RelayCommand] private void GoDashboard() => CurrentViewModel = _dashboard;
        [RelayCommand] private void GoWatchlist() => CurrentViewModel = _watchlist;
        [RelayCommand] private void GoFavorites() => CurrentViewModel = _favorites;
        [RelayCommand] private void GoAlerts()    => CurrentViewModel = _alerts;
        [RelayCommand] private void GoSettings()  => CurrentViewModel = _settings;
    }
}
