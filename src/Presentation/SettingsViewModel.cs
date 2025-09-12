using System;

namespace Presentation
{
    /// <summary>
    /// Minimal SettingsViewModel used to unblock CI builds.
    /// Real wiring for Scheduler/BriefService/Notifier is deferred.
    /// </summary>
    public class SettingsViewModel
    {
        // Example bindable-ish fields to satisfy XAML (no external deps)
        public TimeSpan GenerateTime { get; set; } = new TimeSpan(7, 30, 0);
        public TimeSpan DeliverTime  { get; set; } = new TimeSpan(8,  0, 0);
        public bool AutoStart { get; set; } = false;

        public SettingsViewModel()
        {
            // Intentionally empty; avoids referencing services during CI.
        }

        // Stub methods so bindings/commands (if any) can compile.
        public void Save() { /* no-op for CI */ }
        public void RunOnce() { /* no-op for CI */ }
    }
}
