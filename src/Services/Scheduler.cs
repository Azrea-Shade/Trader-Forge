using Services;
using System;
using System.Globalization;
using Domain;
using Infrastructure;

namespace Services
{
    public class ScheduleSettings
    {
        public TimeSpan GenerateAt { get; init; } = new(7,30,0); // default 07:30
        public TimeSpan NotifyAt   { get; init; } = new(8, 0,0); // default 08:00
        public bool Autostart { get; init; } = true;
    }

    public class Scheduler
    {
        private readonly SqliteDb _db;
        private readonly IClock _clock;
        private readonly BriefService _brief;
        private readonly INotifier _notifier;

        public Scheduler(SqliteDb db, IClock clock, BriefService brief, INotifier notifier)
        {
            _db = db; _clock = clock; _brief = brief; _notifier = notifier;
        }

        // One-shot evaluation; callable in a periodic timer loop by the app.
        // Returns (generated, notified) for observability/testing.
        public (bool generated, bool notified) RunOnce()
        {
            var settings = LoadSettings();
            ApplyAutostart(settings);

            var today = _clock.Today;
            var now   = _clock.Now;

            var lastGen  = GetDate("Brief.LastGenerated");
            var lastNote = GetDate("Brief.LastNotified");

            bool didGen = false, didNote = false;

            // Trigger generate at GenerateAt if not already done today
            var genTs = today.Add(settings.GenerateAt);
            if (now >= genTs && (!lastGen.HasValue || lastGen.Value.Date != today))
            {
                var brief = _brief.Generate();
                // Persist last-generated
                SetDate("Brief.LastGenerated", now);
                didGen = true;

                // Optional: stash last brief text
                SaveBlob("Brief.LastText", brief.SummaryText);
            }

            // Trigger notify at NotifyAt if not already done today
            var noteTs = today.Add(settings.NotifyAt);
            if (now >= noteTs && (!lastNote.HasValue || lastNote.Value.Date != today))
            {
                var msg = LoadBlob("Brief.LastText") ?? "Your daily brief is ready.";
                _notifier.Notify("Azrea Companion — Daily Brief", msg);
                SetDate("Brief.LastNotified", now);
                didNote = true;
            }

            return (didGen, didNote);
        }

        private ScheduleSettings LoadSettings()
        {
            // Format "HH:mm"
            var gen = Get("Brief.GenerateAt") ?? "07:30";
            var not = Get("Brief.NotifyAt") ?? "08:00";
            var auto = (Get("Autostart") ?? "On").Equals("On", StringComparison.OrdinalIgnoreCase);

            TimeSpan TryParse(string s)
            {
                if (TimeSpan.TryParseExact(s, "hh\\:mm", CultureInfo.InvariantCulture, out var t)) return t;
                if (TimeSpan.TryParse(s, out t)) return t;
                return new TimeSpan(7,30,0);
            }

            return new ScheduleSettings { GenerateAt = TryParse(gen), NotifyAt = TryParse(not), Autostart = auto };
        }

        private void ApplyAutostart(ScheduleSettings s)
        {
#if WINDOWS
            // Attempt to set autostart if toggled on
            try
            {
                var exe = Environment.ProcessPath ?? System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName ?? "AzreaCompanion.exe";
                Autostart.Set("AzreaCompanion", exe, s.Autostart);
            }
            catch { /* not fatal */ }
#endif
        }

        // settings helpers
        private string? Get(string key)
        {
            using var cn = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={_db.DbPath}");
            cn.Open();
            using var cmd = cn.CreateCommand();
            cmd.CommandText = "SELECT v FROM settings WHERE k=@k LIMIT 1;";
            cmd.Parameters.AddWithValue("@k", key);
            var o = cmd.ExecuteScalar();
            return o == null || o is DBNull ? null : (string)o;
        }

        private void Set(string key, string value)
        {
            using var cn = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={_db.DbPath}");
            cn.Open();
            using var cmd = cn.CreateCommand();
            cmd.CommandText = @"INSERT INTO settings(k,v) VALUES(@k,@v)
                                ON CONFLICT(k) DO UPDATE SET v=excluded.v;";
            cmd.Parameters.AddWithValue("@k", key);
            cmd.Parameters.AddWithValue("@v", value);
            cmd.ExecuteNonQuery();
        }

        private DateTime? GetDate(string key)
        {
            var s = Get(key);
            if (DateTime.TryParse(s, null, DateTimeStyles.AssumeLocal, out var dt)) return dt;
            return null;
        }
        private void SetDate(string key, DateTime value) => Set(key, value.ToString("o"));

        private void SaveBlob(string key, string value) => Set(key, value);
        private string? LoadBlob(string key) => Get(key);
    }
}
