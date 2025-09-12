using System;
using System.Diagnostics;
using Services;

namespace Presentation
{
    /// <summary>
    /// CI-safe notifier. Implements Services.INotifier.Notify(level,message).
    /// Also exposes Info/Warn/Error helpers for older call sites.
    /// </summary>
    public sealed class WindowsToastNotifier : INotifier
    {
        public void Notify(string level, string message)
        {
            string safeLevel = string.IsNullOrWhiteSpace(level) ? "INFO" : level.ToUpperInvariant();
            var line = $"[{DateTimeOffset.Now:HH:mm:ss}] [{safeLevel}] {message}";
            Debug.WriteLine(line);
            Console.WriteLine(line);
        }

        // Back-compat helpers (fine even if not in the interface)
        public void Info(string text)  => Notify("INFO",  text);
        public void Warn(string text)  => Notify("WARN",  text);
        public void Error(string text) => Notify("ERROR", text);
    }
}
