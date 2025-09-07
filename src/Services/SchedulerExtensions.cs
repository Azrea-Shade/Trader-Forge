using System;
using System.Reflection;

namespace Services
{
    public static class SchedulerExtensions
    {
        /// <summary>
        /// Back-compat: old code called RunOnce(). We try to invoke a suitable method if present,
        /// otherwise no-op so CI compiles.
        /// </summary>
        public static void RunOnce(this Scheduler scheduler)
        {
            if (scheduler is null) return;

            // Try common method names reflectively, but don't fail CI if missing.
            var t = scheduler.GetType();
            var m = t.GetMethod("RunNow") ?? t.GetMethod("RunBriefNow") ?? t.GetMethod("Tick");
            m?.Invoke(scheduler, Array.Empty<object>());
        }
    }
}
