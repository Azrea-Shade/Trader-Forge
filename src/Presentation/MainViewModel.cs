using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace Presentation
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string title = "Dashboard — Phase 1 (Data Foundation)";

        [ObservableProperty]
        private int watchCount;

        [ObservableProperty]
        private int favoriteCount;

        [ObservableProperty]
        private string dbPath = string.Empty;

        public ObservableCollection<string> BriefItems { get; } = new();

        private readonly Services.BriefingService _brief;
        private readonly Services.WatchlistService _watch;
        private readonly Services.FavoritesService _favs;
        private readonly Services.AppPathsService _paths;

        public MainViewModel(Services.BriefingService brief,
                             Services.WatchlistService watch,
                             Services.FavoritesService favs,
                             Services.AppPathsService paths)
        {
            _brief = brief;
            _watch = watch;
            _favs = favs;
            _paths = paths;

            foreach (var s in _brief.GetMorningBriefStub())
                BriefItems.Add(s);

            WatchCount = _watch.GetCount();
            FavoriteCount = _favs.Count();
            DbPath = _paths.DbPath;
        }

        [RelayCommand]
        private void AddMsft()
        {
            WatchCount = _watch.AddSampleMsft();
        }

        [RelayCommand]
        private void CopyDbPath()
        {
            try
            {
                Clipboard.SetText(DbPath ?? "");
            }
            catch { /* ignore */ }
        }
    }
}
