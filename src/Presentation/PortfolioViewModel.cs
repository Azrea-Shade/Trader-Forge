using Presentation;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Domain;
using Services;

namespace Presentation
{
    public class PortfolioViewModel : INotifyPropertyChanged
    {
        private readonly PortfolioService _svc;

        public ObservableCollection<Portfolio> Portfolios { get; } = new();
        public ObservableCollection<Holding> Holdings { get; } = new();
        public ObservableCollection<AllocationRow> Allocation { get; } = new();

        private Portfolio? _selected;
        public Portfolio? Selected
        {
            get => _selected;
            set { _selected = value; OnPropertyChanged(); Reload(); }
        }

        private PortfolioSummary? _summary;
        public PortfolioSummary? Summary { get => _summary; set { _summary = value; OnPropertyChanged(); } }

        // Input fields for adding/updating
        public string NewTicker { get; set; } = "";
        public double NewShares { get; set; }
        public double NewCost { get; set; }

        public ICommand AddHoldingCmd { get; }
        public ICommand RemoveHoldingCmd { get; }
        public ICommand RefreshCmd { get; }

        public PortfolioViewModel(PortfolioService svc)
        {
            _svc = svc;
            AddHoldingCmd = new RelayCommand(_ => AddHolding(), _ => Selected != null && !string.IsNullOrWhiteSpace(NewTicker) && NewShares > 0);
            RemoveHoldingCmd = new RelayCommand(h =>
            {
                if (h is Holding hh) { _svc.RemoveHolding(hh.Id); Reload(); }
            }, _ => Selected != null);
            RefreshCmd = new RelayCommand(_ => Reload());

            LoadPortfolios();
        }

        private void LoadPortfolios()
        {
            Portfolios.Clear();
            foreach (var p in _svc.AllPortfolios()) Portfolios.Add(p);
            if (Portfolios.Count > 0 && Selected == null) Selected = Portfolios[0];
        }

        private void AddHolding()
        {
            if (Selected == null) return;
            _svc.AddHolding(Selected.Id, NewTicker.Trim().ToUpperInvariant(), NewShares, NewCost);
            NewTicker = ""; NewShares = 0; NewCost = 0;
            OnPropertyChanged(nameof(NewTicker));
            OnPropertyChanged(nameof(NewShares));
            OnPropertyChanged(nameof(NewCost));
            Reload();
        }

        private void Reload()
        {
            Holdings.Clear(); Allocation.Clear(); Summary = null;
            if (Selected == null) return;
            foreach (var h in _svc.Holdings(Selected.Id)) Holdings.Add(h);

            var s = _svc.Summary(Selected.Id);
            Summary = s;
            foreach (var a in s.Allocation) Allocation.Add(a);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
