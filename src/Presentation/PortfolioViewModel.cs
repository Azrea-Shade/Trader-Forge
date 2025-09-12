using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Domain;
using Presentation;

namespace Presentation
{
    public class PortfolioViewModel
    {
        private readonly Services.PortfolioService _service;

        public ObservableCollection<Portfolio> Portfolios { get; } = new();
        public ObservableCollection<AllocationRow> Allocation { get; } = new();

        private Portfolio? _selected;
        public Portfolio? Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                OnSelectedChanged();
            }
        }

        public PortfolioViewModel(Services.PortfolioService service)
        {
            _service = service;

            // Fill portfolios using whatever method is present; prefer AllPortfolios if available,
            // otherwise try a couple of likely names to avoid breaking older service APIs.
            IEnumerable<Portfolio> load()
            {
                var svc = _service;
                var t = svc.GetType();

                // Try AllPortfolios()
                var mAll = t.GetMethod("AllPortfolios", Type.EmptyTypes);
                if (mAll != null)
                {
                    var res = mAll.Invoke(svc, Array.Empty<object?>());
                    if (res is IEnumerable<Portfolio> list) return list;
                }

                // Try GetAllPortfolios()
                var mGetAll = t.GetMethod("GetAllPortfolios", Type.EmptyTypes);
                if (mGetAll != null)
                {
                    var res = mGetAll.Invoke(svc, Array.Empty<object?>());
                    if (res is IEnumerable<Portfolio> list) return list;
                }

                // Try Portfolios()
                var mPortfolios = t.GetMethod("Portfolios", Type.EmptyTypes);
                if (mPortfolios != null)
                {
                    var res = mPortfolios.Invoke(svc, Array.Empty<object?>());
                    if (res is IEnumerable<Portfolio> list) return list;
                }

                // Fallback empty
                return Enumerable.Empty<Portfolio>();
            }

            foreach (var p in load()) Portfolios.Add(p);
            Selected = Portfolios.FirstOrDefault();
        }

        private void OnSelectedChanged()
        {
            Allocation.Clear();
            if (Selected is null) return;

            // Call BuildSummary(int) in the service, but accept any actual return type.
            object? summaryObj = null;
            var t = _service.GetType();
            var mBuild = t.GetMethod("BuildSummary", new[] { typeof(int) }) ??
                         t.GetMethod("BuildSummary", new[] { typeof(long) }) ??
                         t.GetMethod("GetSummary",   new[] { typeof(int) }) ??
                         t.GetMethod("GetSummary",   new[] { typeof(long) });

            if (mBuild != null)
            {
                var key = (object)(Selected.Id is int L ? L : Selected.Id);
                summaryObj = mBuild.Invoke(_service, new[] { key });
            }

            // Harden the type here:
            var summary = summaryObj.ToPortfolioSummaryLoose();
            foreach (var row in summary.Allocation)
                Allocation.Add(row);
        }
    }
}
