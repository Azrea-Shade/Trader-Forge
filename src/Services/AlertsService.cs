using System.Collections.Generic;

namespace Services
{
    public class AlertsService
    {
        private readonly Infrastructure.AlertsRepository _repo;
        public AlertsService(Infrastructure.AlertsRepository repo) => _repo = repo;

        public int Count() => _repo.Count();
        public long Add(string ticker, double? above = null, double? below = null, bool enabled = true)
            => _repo.Add(ticker, above, below, enabled);
        public IEnumerable<Infrastructure.AlertRow> All() => _repo.All();
        public void SetEnabled(long id, bool enabled) => _repo.SetEnabled(id, enabled);
    }
}
