using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Domain;
using Services;
using Infrastructure;
using Microsoft.Data.Sqlite;

namespace Presentation
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private readonly SqliteDb _db;
        private readonly Scheduler _scheduler;
        private readonly IClock _clock;

        private string _generateAt = "07:30";
        private string _notifyAt = "08:00";
        private bool _autostart = true;

        public string GenerateAt { get => _generateAt; set { _generateAt = value; OnPropertyChanged(); } }
        public string NotifyAt   { get => _notifyAt;   set { _notifyAt   = value; OnPropertyChanged(); } }
        public bool Autostart    { get => _autostart;  set { _autostart  = value; OnPropertyChanged(); } }

        public ICommand SaveCmd { get; }

        // Preferred constructor (explicit scheduler for DI)
        public SettingsViewModel(SqliteDb db, Scheduler scheduler, IClock clock)
        {
            _db = db; _scheduler = scheduler; _clock = clock;
            SaveCmd = new RelayCommand(_ => Save());
            Load();
        }

        // Back-compat: 2-arg overload (db + clock)
        public SettingsViewModel(SqliteDb db, IClock clock)
            : this(
                db,
                new Scheduler(
                    db,
                    clock,
                    new BriefService(
                        new WatchlistReader(db),
                        new PortfolioService(new PortfoliosRepository(db), new DummyPriceFeed()),
                        clock),
                    new FileNotifier()),
                clock)
        { }

        // Back-compat: 1-arg overload (db only) — builds SystemClock + default Scheduler
        public SettingsViewModel(SqliteDb db)
            : this(
                db,
                new Scheduler(
                    db,
                    new SystemClock(),
                    new BriefService(
                        new WatchlistReader(db),
                        new PortfolioService(new PortfoliosRepository(db), new DummyPriceFeed()),
                        new SystemClock()),
                    new FileNotifier()),
                new SystemClock())
        { }

        // Extra safety: 1-arg overload (clock only) — creates a shared db instance
        public SettingsViewModel(IClock clock)
            : this(
                new SqliteDb(),
                new Scheduler(
                    new SqliteDb(), // shared local variable to keep same path
                    clock,
                    new BriefService(
                        new WatchlistReader(new SqliteDb()),
                        new PortfolioService(new PortfoliosRepository(new SqliteDb()), new DummyPriceFeed()),
                        clock),
                    new FileNotifier()),
                clock)
        { }

        private void Load()
        {
            GenerateAt = Get("Brief.GenerateAt") ?? "07:30";
            NotifyAt   = Get("Brief.NotifyAt")   ?? "08:00";
            Autostart  = (Get("Autostart") ?? "On").Equals("On", StringComparison.OrdinalIgnoreCase);
        }

        private void Save()
        {
            Set("Brief.GenerateAt", GenerateAt.Trim());
            Set("Brief.NotifyAt",   NotifyAt.Trim());
            Set("Autostart",        Autostart ? "On" : "Off");
            _scheduler.RunOnce();
        }

        private string? Get(string key)
        {
            using var cn = new SqliteConnection($"Data Source={_db.DbPath}");
            cn.Open();
            using var cmd = cn.CreateCommand();
            cmd.CommandText = "SELECT v FROM settings WHERE k=@k LIMIT 1;";
            cmd.Parameters.AddWithValue("@k", key);
            var o = cmd.ExecuteScalar();
            return o == null || o is DBNull ? null : (string)o;
        }

        private void Set(string key, string value)
        {
            using var cn = new SqliteConnection($"Data Source={_db.DbPath}");
            cn.Open();
            using var cmd = cn.CreateCommand();
            cmd.CommandText = @"INSERT INTO settings(k,v) VALUES(@k,@v)
                                ON CONFLICT(k) DO UPDATE SET v=excluded.v;";
            cmd.Parameters.AddWithValue("@k", key);
            cmd.Parameters.AddWithValue("@v", value);
            cmd.ExecuteNonQuery();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? p=null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
    }
}
