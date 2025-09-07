using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Presentation
{
    public class AlertItem
    {
        public long Id { get; init; }
        public string Ticker { get; init; } = "";
        public double? Above { get; init; }
        public double? Below { get; init; }
        public bool Enabled { get; set; }
    }

    public partial class AlertsViewModel : ObservableObject
    {
        [ObservableProperty] private string header = "Alerts";

        [ObservableProperty] private string newTicker = string.Empty;
        [ObservableProperty] private double? newAbove;
        [ObservableProperty] private double? newBelow;

        public ObservableCollection<AlertItem> Items { get; } = new();

        private readonly Services.AlertsService _alerts;

        public AlertsViewModel(Services.AlertsService alerts)
        {
            _alerts = alerts;
            Reload();
        }

        [RelayCommand]
        private void Add()
        {
            if (string.IsNullOrWhiteSpace(NewTicker)) return;
            _alerts.Add(NewTicker, NewAbove, NewBelow, enabled: true);
            NewTicker = ""; NewAbove = null; NewBelow = null;
            Reload();
        }

        [RelayCommand]
        private void Toggle(AlertItem item)
        {
            _alerts.SetEnabled(item.Id, !item.Enabled);
            Reload();
        }

        private void Reload()
        {
            Items.Clear();
            foreach (var r in _alerts.All())
            {
                Items.Add(new AlertItem
                {
                    Id = r.Id, Ticker = r.Ticker, Above = r.Above, Below = r.Below, Enabled = r.Enabled
                });
            }
        }
    }
}
