using System.Diagnostics;
using Services;

namespace Presentation
{
    // Simple, CI-safe toast notifier. Real Windows toast wiring can come later.
    public sealed class WindowsToastNotifier : INotifier
    {
        public void Notify(string title, string message)
        {
            // No dependencies: just log. UI popups can be added in-app on Windows.
            Debug.WriteLine($"[Toast] {title}: {message}");
        }
    }
}
