using Microsoft.Toolkit.Uwp.Notifications;

namespace AzreaCompanion
{
    public class WinToastNotifier : Services.IToastNotifier
    {
        public void ShowInfo(string title, string message)
        {
            // Uses Toolkit's builder (works from Win32/WPF)
            new ToastContentBuilder()
                .AddText(title)
                .AddText(message)
                .Show();
        }
    }
}
