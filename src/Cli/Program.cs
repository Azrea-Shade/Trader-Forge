using Infrastructure;
using Services;
using Domain;

var db = new SqliteDb();
var watch = new WatchlistReader(db);
var repo = new PortfoliosRepository(db);
var prices = new DummyPriceFeed();
var portfolios = new PortfolioService(repo, prices);
var clock = new SystemClock();
var brief = new BriefService(watch, portfolios, clock);
var notifier = new FileNotifier();
var scheduler = new Scheduler(db, clock, brief, notifier);

// Simple args:
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
    // default CLI smoke
    foreach (var p in portfolios.AllPortfolios())
    {
        var s = portfolios.Summary(p.Id);
        Console.WriteLine($"{p.Name}: MV={s.TotalMarketValue:C} P/L={s.GainLoss:C} ({s.GainLossPct:F2}%)");
    }
}
