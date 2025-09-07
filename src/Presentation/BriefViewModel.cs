using System;
using System.Collections.ObjectModel;

namespace Presentation
{
    /// <summary>
    /// Simple VM for the Daily Brief page. Self-contained so Presentation compiles in CI.
    /// Later we can inject a service to populate real data.
    /// </summary>
    public sealed class BriefViewModel : ViewModelBase
    {
        private DateTime _generatedAtLocal = DateTime.Now;
        public DateTime GeneratedAtLocal
        {
            get => _generatedAtLocal;
            set => Set(ref _generatedAtLocal, value);
        }

        private string _summaryText = "Your daily brief will appear here.";
        public string SummaryText
        {
            get => _summaryText;
            set => Set(ref _summaryText, value);
        }

        /// <summary>Tickers to display; used by BriefView.xaml ItemsControl.</summary>
        public ObservableCollection<string> Tickers { get; } = new();
    }
}
