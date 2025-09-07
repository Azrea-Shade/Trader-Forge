using System;
using System.IO;
using Services; // INotifier

namespace Presentation
{
    /// <summary>
    /// CI-safe notifier that satisfies INotifier's Notify(level,message) contract.
    /// Also exposes Info/Warn/Error for convenience. Writes to console + local file.
    /// </summary>
    public sealed class WindowsToastNotifier : INotifier
    {
        // Interface member required by CI: Notify(string level, string message)
        public void Notify(string level, string message)
        {
            if (string.IsNullOrWhiteSpace(level)) level = "INFO";
            Log(level.ToUpperInvariant(), message ?? string.Empty);
        }

        // Convenience helpers (in case app uses them)
        public void Info(string message)  => Notify("INFO",  message);
        public void Warn(string message)  => Notify("WARN",  message);
        public void Error(string message) => Notify("ERROR", message);

        private static void Log(string level, string message)
        {
            var line = $"[{DateTimeOffset.Now:O}] {level}: {message}{Environment.NewLine}";
            try
            {
                var path = Path.Combine(AppContext.BaseDirectory, "notify.log");
                File.AppendAllText(path, line);
            }
            catch { /* ignore on CI */ }

            try { Console.WriteLine($"[Toast/{level}] {message}"); } catch { /* ignore */ }
        }
    }
}
