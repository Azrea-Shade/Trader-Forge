using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Presentation
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string title = "Dashboard — Testing Branch";

        [ObservableProperty]
        private int watchCount;

        public ObservableCollection<string> BriefItems { get; } = new();

        private readonly Services.BriefingService _brief;
        private readonly Services.WatchlistService _watch;

        public MainViewModel(Services.BriefingService brief, Services.WatchlistService watch)
        {
            _brief = brief;
            _watch = watch;

            foreach (var s in _brief.GetMorningBriefStub())
                BriefItems.Add(s);

            WatchCount = _watch.GetCount();
        }

        [RelayCommand]
        private void AddMsft()
        {
            WatchCount = _watch.AddSampleMsft();
        }
    }
}
