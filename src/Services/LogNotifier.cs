using System;

namespace Services
{
    /// <summary>Console logger notifier (CI-safe, cross-platform).</summary>
    public sealed class LogNotifier : INotifier
    {
        public void Notify(string title, string message)
        {
            Console.WriteLine($"[NOTIFY] {title}: {message}");
        }
    }
}
