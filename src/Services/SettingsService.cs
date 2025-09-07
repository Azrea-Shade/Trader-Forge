using Domain;
using Infrastructure;

namespace Services
{
    public class SettingsService
    {
        private readonly SettingsRepository _repo;
        public SettingsService(SettingsRepository repo) => _repo = repo;

        public AppSettings Load() => _repo.LoadOrDefault();
        public void Save(AppSettings s) => _repo.Save(s);
    }
}
