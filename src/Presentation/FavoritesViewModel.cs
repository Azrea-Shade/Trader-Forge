using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Presentation
{
    public partial class FavoritesViewModel : ObservableObject
    {
        [ObservableProperty] private string header = "Favorites";
        [ObservableProperty] private string newTicker = string.Empty;
        [ObservableProperty] private string? selectedTicker;

        public ObservableCollection<string> Items { get; } = new();

        private readonly Services.FavoritesService _favs;

        public FavoritesViewModel(Services.FavoritesService favs)
        {
            _favs = favs;
            Reload();
        }

        [RelayCommand]
        private void Add()
        {
            if (!string.IsNullOrWhiteSpace(NewTicker))
            {
                _favs.Add(NewTicker);
                NewTicker = "";
                Reload();
            }
        }

        [RelayCommand]
        private void RemoveSelected()
        {
            if (!string.IsNullOrWhiteSpace(SelectedTicker))
            {
                _favs.Remove(SelectedTicker);
                Reload();
            }
        }

        private void Reload()
        {
            Items.Clear();
            foreach (var t in _favs.All())
                Items.Add(t);
        }
    }
}
