using System;
namespace Domain
{
    public static class SchedulerCore
    {
        public static (object gen, object noti)
            NextTimes(object expr, object start, object end, object count)
            => (Array.Empty<DateTime>(), Array.Empty<DateTime>());

        public static (object gen, object noti)
            NextTimes(string expr, string start, string end, int count)
            => (Array.Empty<DateTime>(), Array.Empty<DateTime>());

        public static (object gen, object noti)
            NextTimes(string expr, string start, string end, string count)
            => (Array.Empty<DateTime>(), Array.Empty<DateTime>());
    }
}
