using System;
using System.IO;
using Services; // INotifier

namespace Presentation
{
    /// <summary>
    /// CI-safe notifier used by the WPF app. Implements INotifier (Info/Warn/Error).
    /// On CI (no toast channel), we just log to console + a local file.
    /// You can later swap the body for real Windows toast calls.
    /// </summary>
    public sealed class WindowsToastNotifier : INotifier
    {
        public void Info(string message)  => Log("INFO",  message);
        public void Warn(string message)  => Log("WARN",  message);
        public void Error(string message) => Log("ERROR", message);

        private static void Log(string level, string message)
        {
            var line = $"[{DateTimeOffset.Now:O}] {level}: {message}{Environment.NewLine}";
            try
            {
                // Write beside the binaries to avoid permission issues on CI
                var path = Path.Combine(AppContext.BaseDirectory, "notify.log");
                File.AppendAllText(path, line);
            }
            catch { /* swallow for CI */ }
            try { Console.WriteLine($"[Toast/{level}] {message}"); } catch { /* ignore */ }
        }
    }
}
