using System;
using System.IO;
using Services;

namespace Presentation
{
    /// <summary>
    /// Minimal notifier with zero external dependencies.
    /// Writes notifications to a log file so CI/tests compile & run clean.
    /// Runtime Windows toast integration will be added in a later phase.
    /// </summary>
    public sealed class WindowsToastNotifier : INotifier
    {
        private readonly string _appId;
        private readonly string _title;
        private readonly string _shortcutName;
        private readonly string _logPath;

        public WindowsToastNotifier(string appId, string title, string? shortcutName = null)
        {
            _appId = appId;
            _title = title;
            _shortcutName = shortcutName ?? "Trader Forge.lnk";

            var local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var dir = Path.Combine(local, "TraderForge");
            Directory.CreateDirectory(dir);
            _logPath = Path.Combine(dir, "toasts.log");
        }

        public void Notify(string title, string message)
        {
            try
            {
                var line = $"{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss} [{_appId}] {(string.IsNullOrWhiteSpace(title)?_title:title)} :: {message}";
                File.AppendAllText(_logPath, line + Environment.NewLine);
            }
            catch
            {
                // Swallow to keep scheduler resilient
            }
        }
    }
}
