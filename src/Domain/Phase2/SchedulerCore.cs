using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public static class SchedulerCore
    {
        public static (object gen, object noti)
            NextTimes(object expr, object start, object end, object count)
            => (Enumerable.Empty<DateTime>(), Enumerable.Empty<DateTime>());

        public static (object gen, object noti)
            NextTimes(string expr, string start, string end, int count)
            => (Enumerable.Empty<DateTime>(), Enumerable.Empty<DateTime>());

        public static (object gen, object noti)
            NextTimes(string expr, string start, string end, string count)
            => (Enumerable.Empty<DateTime>(), Enumerable.Empty<DateTime>());
    }
}
