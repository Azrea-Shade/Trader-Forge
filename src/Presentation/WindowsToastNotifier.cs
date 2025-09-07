using System;
using System.Diagnostics;
using Services;

namespace Presentation
{
    /// <summary>
    /// CI-safe notifier used by WPF layer. Implements the current INotifier contract
    /// (Info/Warn/Error). Real Windows toast UX can be layered on later.
    /// </summary>
    public sealed class WindowsToastNotifier : INotifier
    {
        private static void Log(string level, string text)
        {
            var line = $"[{level}] {text}";
            Debug.WriteLine(line);
            Console.WriteLine(line);
        }

        public void Info(string text)  => Log("INFO",  text);
        public void Warn(string text)  => Log("WARN",  text);
        public void Error(string text) => Log("ERROR", text);
    }
}
