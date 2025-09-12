using System;

namespace Services
{
    /// <summary>
    /// Unified notification contract used across the app and CI-safe code paths.
    /// </summary>
    public interface INotifier
    {
        // Single-message helpers
        void Info(string message);
        void Warn(string message);
        void Error(string message);

        // Two-argument general notification (title + message)
        void Notify(string title, string message);
    }
}
