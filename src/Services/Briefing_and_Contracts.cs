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

    public class WatchlistService
    {
        private readonly Infrastructure.WatchlistRepository _repo;
        public WatchlistService(Infrastructure.WatchlistRepository repo) => _repo = repo;

        public int GetCount() => _repo.Count();
        public int AddSampleMsft() { _repo.Add("MSFT"); return _repo.Count(); }
    }
}
