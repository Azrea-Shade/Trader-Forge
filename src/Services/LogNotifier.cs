using System;

namespace Services
{
    /// <summary>
    /// Console logger notifier (no file I/O). CI-safe.
    /// </summary>
    public sealed class LogNotifier : INotifier
    {
        public void Info(string message)  => Console.WriteLine($"[INFO] {message}");
        public void Warn(string message)  => Console.WriteLine($"[WARN] {message}");
        public void Error(string message) => Console.Error.WriteLine($"[ERROR] {message}");
        public void Notify(string title, string message)
            => Console.WriteLine($"[{(string.IsNullOrWhiteSpace(title) ? "NOTICE" : title)}] {message}");
    }
}
