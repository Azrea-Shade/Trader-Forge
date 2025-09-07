using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Presentation
{
    public partial class SettingsViewModel : ObservableObject
    {
        [ObservableProperty] private string header = "Settings";
        [ObservableProperty] private string generateAt = "07:30";
        [ObservableProperty] private string notifyAt = "08:00";
        [ObservableProperty] private string autostart = "On";

        private readonly Services.SettingsService _settings;

        public SettingsViewModel(Services.SettingsService settings)
        {
            _settings = settings;
            var d = _settings.GetBriefDefaults();
            GenerateAt = d.GenerateAt;
            NotifyAt  = d.NotifyAt;
            Autostart = d.Autostart;
        }

        [RelayCommand]
        private void Save()
        {
            _settings.Set("Brief.GenerateAt", GenerateAt);
            _settings.Set("Brief.NotifyAt",  NotifyAt);
            _settings.Set("Autostart",       Autostart);
        }
    }
}
