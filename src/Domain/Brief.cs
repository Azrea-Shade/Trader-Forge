using System.Linq;
using System;
using System.Collections.Generic;

namespace Domain
{
    public interface IClock
    {
        DateTime Now { get; }
        DateTime UtcNow { get; }
        DateTime Today { get; }
    }

    public sealed class SystemClock : IClock
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
        public DateTime Today => DateTime.Today;
    }

    public class DailyBrief
    {
        public DateTime GeneratedAtLocal { get; set; }
        public string SummaryText { get; set; } = "";
        public string[] Tickers { get; set; } = Array.Empty<string>();
    }
}
