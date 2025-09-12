using Services;
namespace Services
{
    public class SettingsService
    {
        private readonly Infrastructure.SettingsRepository _repo;
        public SettingsService(Infrastructure.SettingsRepository repo) => _repo = repo;

        public string? Get(string key) => _repo.Get(key);
        public void Set(string key, string value) => _repo.Set(key, value);

        public (string GenerateAt, string NotifyAt, string Autostart) GetBriefDefaults()
            => (_repo.Get("Brief.GenerateAt") ?? "07:30",
                _repo.Get("Brief.NotifyAt") ?? "08:00",
                _repo.Get("Autostart") ?? "On");
    }
}
