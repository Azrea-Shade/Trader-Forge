using CommunityToolkit.Mvvm.ComponentModel;

namespace Presentation
{
    public partial class DashboardViewModel : ObservableObject
    {
        [ObservableProperty] private string header = "Dashboard";
        [ObservableProperty] private string dbPath = string.Empty;

        public DashboardViewModel(Services.AppPathsService paths)
        {
            DbPath = paths.DbPath;
        }
    }
}
