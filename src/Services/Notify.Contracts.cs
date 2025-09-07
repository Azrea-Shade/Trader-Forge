using System;
using System.IO;

namespace Services
{
    public interface INotifier
    {
        void Info(string message);
        void Warn(string message);
        void Error(string message);
    }

    /// <summary>Simple notifier that writes to a file (CI-safe).</summary>
    public sealed class FileNotifier : INotifier
    {
        private readonly string _path;
        public FileNotifier(string path)
        {
            _path = string.IsNullOrWhiteSpace(path) ? "artifacts/brief.log" : path;
            var dir = Path.GetDirectoryName(_path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
        }
        private void Write(string level, string msg)
        {
            var line = $"[{DateTime.Now:O}] {level}: {msg}{Environment.NewLine}";
            File.AppendAllText(_path, line);
        }
        public void Info(string message)  => Write("INFO",  message);
        public void Warn(string message)  => Write("WARN",  message);
        public void Error(string message) => Write("ERROR", message);
    }

    /// <summary>Clock contract + system implementation used by Scheduler/Briefing.</summary>
    public interface IClock { DateTime Now { get; } }
    public sealed class SystemClock : IClock { public DateTime Now => DateTime.Now; }
}
