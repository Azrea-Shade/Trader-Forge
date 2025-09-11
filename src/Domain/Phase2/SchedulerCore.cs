using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public static class SchedulerCore
    {
        // Minimal signature to satisfy tuple-deconstruction tests.
        public static (IEnumerable<DateTime> gen, IEnumerable<DateTime> noti)
            NextTimes(DateTime from, object schedule)
        {
            return (Enumerable.Empty<DateTime>(), Enumerable.Empty<DateTime>());
        }
    }
}
