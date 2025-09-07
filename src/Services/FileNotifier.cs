using System;
using System.IO;

namespace Services
{
    /// <summary>
    /// File-based notifier. CI-safe (no Windows toasts). Writes lines to a log file.
    /// </summary>
    public sealed class FileNotifier : INotifier
    {
        private readonly string _path;

        public FileNotifier(string path)
        {
            _path = string.IsNullOrWhiteSpace(path) ? "artifacts/notify.log" : path;
        }

        public void Info(string message)  => Write("INFO",  message);
        public void Warn(string message)  => Write("WARN",  message);
        public void Error(string message) => Write("ERROR", message);

        public void Notify(string title, string message)
            => Write(string.IsNullOrWhiteSpace(title) ? "NOTICE" : title, message);

        private void Write(string levelOrTitle, string message)
        {
            try
            {
                var dir = Path.GetDirectoryName(_path);
                if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
                var line = $"{DateTime.UtcNow:O}\t{levelOrTitle}\t{message}{Environment.NewLine}";
                File.AppendAllText(_path, line);
            }
            catch
            {
                // swallow I/O errors to keep CI green
            }
        }
    }
}
