using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Presentation
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string title = "Dashboard — Testing Branch";

        public ObservableCollection<string> BriefItems { get; } = new();

        public MainViewModel(Services.BriefingService brief)
        {
            foreach (var s in brief.GetMorningBriefStub())
                BriefItems.Add(s);
        }
    }
}
