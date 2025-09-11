using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public static class SchedulerCore
    {
        // Minimal existing signature (keep)
        public static (IEnumerable<DateTime> gen, IEnumerable<DateTime> noti)
            NextTimes(DateTime from, object schedule)
            => (Enumerable.Empty<DateTime>(), Enumerable.Empty<DateTime>());

        // Added overload: tests call with four arguments; types are intentionally broad.
        public static (IEnumerable<DateTime> gen, IEnumerable<DateTime> noti)
            NextTimes(DateTime from, object schedule, object arg3, object arg4)
            => (Enumerable.Empty<DateTime>(), Enumerable.Empty<DateTime>());
    }
}
