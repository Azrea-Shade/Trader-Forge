// Phase 5: minimal CLI to smoke PortfolioService without UI (used by CI smoke if needed)
using Infrastructure;
using Services;

var db = new SqliteDb();
var repo = new PortfoliosRepository(db);
var prices = new DummyPriceFeed();
var svc = new PortfolioService(repo, prices);

foreach (var p in svc.AllPortfolios())
{
    var s = svc.Summary(p.Id);
    Console.WriteLine($"{p.Name}: MV={s.TotalMarketValue:C} P/L={s.GainLoss:C} ({s.GainLossPct:F2}%)");
}
