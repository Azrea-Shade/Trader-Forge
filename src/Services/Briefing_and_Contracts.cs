using System.Collections.Generic;

namespace Services
{
    public class BriefingService
    {
        public IEnumerable<string> GetMorningBriefStub()
        {
            yield return "Market overview at 8:00 AM (stub)";
            yield return "Watchlist summaries (stub)";
            yield return "Reminders & tasks for today (stub)";
        }
    }
}
