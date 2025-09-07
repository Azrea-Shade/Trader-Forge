using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Services.Feeds;
using Services.Engines;

namespace Presentation
{
    public partial class WatchlistViewModel : ObservableObject
    {
        public class Row : ObservableObject
        {
            public long Id { get; set; }
            [ObservableProperty] private string ticker = "";
            [ObservableProperty] private double? above;
            [ObservableProperty] private double? below;
            [ObservableProperty] private double? price;
            [ObservableProperty] private string triggered = "";
        }

        [ObservableProperty] private string header = "Watchlist";
        [ObservableProperty] private string newTicker = string.Empty;
        [ObservableProperty] private string lastUpdated = "—";
        [ObservableProperty] private Row? selected;

        public ObservableCollection<Row> Items { get; } = new();

        private readonly Services.WatchlistFacade _facade;
        private readonly IPriceFeed _priceFeed;

        public WatchlistViewModel(Services.WatchlistFacade facade, IPriceFeed priceFeed)
        {
            _facade = facade;
            _priceFeed = priceFeed;
            Reload();
        }

        private void Reload()
        {
            Items.Clear();
            foreach (var r in _facade.All())
            {
                Items.Add(new Row { Id = r.Id, Ticker = r.Ticker, Above = r.Above, Below = r.Below });
            }
        }

        [RelayCommand]
        private void Add()
        {
            if (string.IsNullOrWhiteSpace(NewTicker)) return;
            var id = _facade.Add(NewTicker.Trim().ToUpperInvariant());
            NewTicker = "";
            Reload();
            Selected = Items.FirstOrDefault(x => x.Id == id);
        }

        [RelayCommand]
        private void RemoveSelected()
        {
            if (Selected is null) return;
            _facade.Remove(Selected.Id);
            Selected = null;
            Reload();
        }

        [RelayCommand]
        private void SaveAll()
        {
            foreach (var r in Items)
                _facade.SetThresholds(r.Id, r.Above, r.Below);
        }

        [RelayCommand]
        private async Task RefreshPrices()
        {
            var tickers = Items.Select(x => x.Ticker).Distinct().ToArray();
            if (tickers.Length == 0) return;
            var prices = await _priceFeed.GetPricesAsync(tickers);
            foreach (var row in Items)
                row.Price = prices.TryGetValue(row.Ticker, out var p) ? p : null;
            LastUpdated = System.DateTime.Now.ToString("HH:mm");
        }

        [RelayCommand]
        private async Task EvaluateAlertsNow()
        {
            // Ensure prices exist
            if (Items.All(i => i.Price is null))
                await RefreshPrices();

            var latest = Items.Where(i => i.Price is not null)
                              .ToDictionary(i => i.Ticker, i => i.Price!.Value,
                                            System.StringComparer.OrdinalIgnoreCase);

            var rules = Items.Select(i =>
                new Infrastructure.AlertRow(i.Id, i.Ticker, i.Above, i.Below, true)).ToArray();

            var evals = AlertEngine.Evaluate(rules, latest)
                                   .ToDictionary(e => e.Id);

            foreach (var row in Items)
            {
                if (!evals.TryGetValue(row.Id, out var e))
                {
                    row.Triggered = "";
                    continue;
                }

                row.Triggered = e.TriggeredAbove ? "Above"
                              : e.TriggeredBelow ? "Below"
                              : "None";
            }
        }
    }
}
