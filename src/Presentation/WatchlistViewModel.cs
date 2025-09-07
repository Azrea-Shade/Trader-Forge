using CommunityToolkit.Mvvm.ComponentModel;

namespace Presentation
{
    public partial class WatchlistViewModel : ObservableObject
    {
        [ObservableProperty] private string header = "Watchlist";
        [ObservableProperty] private string note = "Watchlist wiring arrives in Phase 4 (add/remove, thresholds, prices).";
    }
}
