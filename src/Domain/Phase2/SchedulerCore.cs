using System;

namespace Domain
{
    public static class SchedulerCore
    {
        public static (DateTime gen, DateTime noti)
            NextTimes(string cron, DateTime from)
            => (from, from);

        public static (DateTime gen, DateTime noti)
            NextTimes(string cron, DateTime from, object arg3, object arg4)
            => (from, from);
    }
}
