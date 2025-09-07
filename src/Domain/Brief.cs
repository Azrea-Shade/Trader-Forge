using System;

namespace Domain
{
    // Simple POCO used by Services + tests
    public class DailyBrief
    {
        public DateOnly Date { get; set; }
        public string SummaryText { get; set; } = "";
        public string[] Tickers { get; set; } = Array.Empty<string>();
        public DateTimeOffset GeneratedAtLocal { get; set; }
    }
}
