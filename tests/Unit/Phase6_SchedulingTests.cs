using System;
using System.IO;
using Domain;
using Infrastructure;
using Services;
using Xunit;
using FluentAssertions;

public class Phase6_SchedulingTests
{
    private sealed class ManualClock : IClock
    {
        private DateTime _now;
        public ManualClock(DateTime start) { _now = start; }
        public DateTime Now => _now;
        public DateTime UtcNow => _now.ToUniversalTime();
        public DateTime Today => _now.Date;
        public void AdvanceMinutes(int m) => _now = _now.AddMinutes(m);
        public void Set(DateTime t) => _now = t;
    }

    private static string TempDb()
    {
        var p = Path.Combine(Path.GetTempPath(), $"azrea_phase6_{Guid.NewGuid():N}.db");
        if (File.Exists(p)) File.Delete(p);
        return p;
    }

    [Fact]
    public void Generates_then_notifies_once_per_day_at_times()
    {
        var dbfile = TempDb();
        var db = new SqliteDb(dbfile);
        var repo = new PortfoliosRepository(db);
        var prices = new DummyPriceFeed();
        var ports = new PortfolioService(repo, prices);
        var watch = new WatchlistReader(db);

        // Some seed data
        var pid = ports.CreatePortfolio("P1");
        ports.AddHolding(pid, "AAPL", 1, 100);

        var start = new DateTime(2025, 1, 1, 7, 0, 0);
        var clock = new ManualClock(start);
        var notifier = new FileNotifier(Path.Combine(Path.GetTempPath(), $"notif_{Guid.NewGuid():N}.log"));
        var brief = new BriefService(watch, ports, clock);
        var sched = new Scheduler(db, clock, brief, notifier);

        // set settings explicitly
        using (var cn = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={db.DbPath}"))
        {
            cn.Open();
            using var cmd = cn.CreateCommand();
            cmd.CommandText = @"INSERT INTO settings(k,v) VALUES('Brief.GenerateAt','07:30'),
                                                           ('Brief.NotifyAt','08:00')
                                ON CONFLICT(k) DO UPDATE SET v=excluded.v;";
            cmd.ExecuteNonQuery();
        }

        // Before 07:30 -> nothing
        var r0 = sched.RunOnce();
        r0.generated.Should().BeFalse();
        r0.notified.Should().BeFalse();

        // At 07:30 -> generate only
        clock.Set(new DateTime(2025, 1, 1, 7, 30, 0));
        var r1 = sched.RunOnce();
        r1.generated.Should().BeTrue();
        r1.notified.Should().BeFalse();

        // Between 07:30 and 08:00 -> no repetition
        clock.Set(new DateTime(2025, 1, 1, 7, 45, 0));
        var r2 = sched.RunOnce();
        r2.generated.Should().BeFalse();
        r2.notified.Should().BeFalse();

        // At 08:00 -> notify once
        clock.Set(new DateTime(2025, 1, 1, 8, 0, 0));
        var r3 = sched.RunOnce();
        r3.generated.Should().BeFalse();
        r3.notified.Should().BeTrue();

        // Later same day -> no repeats
        clock.Set(new DateTime(2025, 1, 1, 9, 0, 0));
        var r4 = sched.RunOnce();
        r4.generated.Should().BeFalse();
        r4.notified.Should().BeFalse();

        // Next day -> generate again at 07:30
        clock.Set(new DateTime(2025, 1, 2, 7, 30, 0));
        var r5 = sched.RunOnce();
        r5.generated.Should().BeTrue();
        r5.notified.Should().BeFalse();
    }
}
