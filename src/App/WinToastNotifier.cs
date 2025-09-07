using Microsoft.Toolkit.Uwp.Notifications;

namespace AzreaCompanion
{
    public class WinToastNotifier : Services.IToastNotifier
    {
        public void ShowInfo(string title, string message)
        {
            new ToastContentBuilder()
                .AddText(title)
                .AddText(message)
                .Show();
        }
    }
}
