using Services;
using System;

namespace Services.Engines
{
    public static class SchedulerCore
    {
        /// <summary>
        /// Compute the next "generate" and "notify" DateTimes from HH:mm strings in a timezone.
        /// </summary>
        public static (DateTime nextGenerate, DateTime nextNotify) NextTimes(
            string generateAtHHmm, string notifyAtHHmm, DateTime nowLocal, TimeZoneInfo tz)
        {
            static DateTime NextAt(string hhmm, DateTime now)
            {
                var parts = hhmm.Split(':');
                if (parts.Length != 2) throw new ArgumentException("Time must be HH:mm", nameof(hhmm));
                var h = int.Parse(parts[0]); var m = int.Parse(parts[1]);
                var candidate = new DateTime(now.Year, now.Month, now.Day, h, m, 0);
                return candidate <= now ? candidate.AddDays(1) : candidate;
            }

            var gen = NextAt(generateAtHHmm, nowLocal);
            var noti = NextAt(notifyAtHHmm, nowLocal);

            return (gen, noti);
        }
    }
}
