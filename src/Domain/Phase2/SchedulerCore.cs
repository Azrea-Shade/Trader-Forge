using System;
using System.Globalization;

namespace Domain
{
    public static class SchedulerCore
    {
        private static DateTime ParseUtc(string s)
        {
            if (DateTime.TryParse(s, CultureInfo.InvariantCulture,
                                  DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal,
                                  out var dt))
                return dt;
            return DateTime.UtcNow;
        }

        public static (DateTime gen, DateTime noti)
            NextTimes(string cron, DateTime from)
            => (from, from);

        public static (DateTime gen, DateTime noti)
            NextTimes(string cron, DateTime from, object arg3, object arg4)
            => (from, from);

        public static (DateTime gen, DateTime noti)
            NextTimes(string cron, string from)
            => NextTimes(cron, ParseUtc(from));

        public static (DateTime gen, DateTime noti)
            NextTimes(string cron, string from, object arg3, object arg4)
            => NextTimes(cron, ParseUtc(from), arg3, arg4);
    }
}
