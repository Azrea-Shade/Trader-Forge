using Infrastructure;
using Services;
using Domain;
using System;
using Microsoft.Data.Sqlite;
using Presentation;

static bool IsWindows()
{
    return System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);
}

static bool ToastsEnabled(SqliteDb db)
{
    using var cn = new SqliteConnection($"Data Source={"${db.DbPath}"}");
    cn.Open();
    using var cmd = cn.CreateCommand();
    cmd.CommandText = "SELECT v FROM settings WHERE k='Notifications.Toasts' LIMIT 1;";
    var o = cmd.ExecuteScalar() as string;
    return string.IsNullOrWhiteSpace(o) || o.Equals("On", StringComparison.OrdinalIgnoreCase);
}

var db = new SqliteDb();
var watch = new WatchlistReader(db);
var repo = new PortfoliosRepository(db);
var prices = new DummyPriceFeed();
var portfolios = new PortfolioService(repo, prices);
var clock = new SystemClock();

INotifier notifier = new FileNotifier();
if (IsWindows() && ToastsEnabled(db))
{
    // Use real Windows toasts with the agreed AppID and title
    notifier = new WindowsToastNotifier("com.azrea.traderforge", "Trader Forge Tracker", "Trader Forge Companion.lnk");
}

var brief = new BriefService(watch, portfolios, clock);
var scheduler = new Scheduler(db, clock, brief, notifier);

// CLI options:
//   --brief      : generate and print today's brief
//   --schedule   : run one-shot scheduler evaluation (generate/notify if due)
if (args.Length > 0 && args[0] == "--brief")
{
    var b = brief.Generate();
    Console.WriteLine(b.SummaryText);
}
else if (args.Length > 0 && args[0] == "--schedule")
{
    var (gen, note) = scheduler.RunOnce();
    Console.WriteLine($"Schedule ran. Generated={gen}, Notified={note}");
}
else
{
    foreach (var p in portfolios.AllPortfolios())
    {
        var s = portfolios.Summary(p.Id);
        Console.WriteLine($"{p.Name}: MV={s.TotalMarketValue:C} P/L={s.GainLoss:C} ({s.GainLossPct:F2}%)");
    }
}
