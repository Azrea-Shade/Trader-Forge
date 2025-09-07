using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Infrastructure;
using Services.Feeds;

namespace Services
{
    public class BriefingService
    {
        private readonly SqliteDb _db;
        private readonly BriefsRepository _briefs;
        private readonly IPriceFeed _prices;

        public BriefingService(SqliteDb db, IPriceFeed prices)
        {
            _db = db;
            _briefs = new BriefsRepository(db);
            _prices = prices;
        }

        public async Task<string> GenerateAsync(DateOnly date)
        {
            var content = await BuildContentAsync();
            _briefs.UpsertGenerated(date.ToString("yyyy-MM-dd"), content, DateTimeOffset.UtcNow);
            return content;
        }

        public void MarkDelivered(DateOnly date) =>
            _briefs.MarkDelivered(date.ToString("yyyy-MM-dd"), DateTimeOffset.UtcNow);

        private async Task<string> BuildContentAsync()
        {
            using var cn = new SqliteConnection($"Data Source={_db.DbPath}");
            await cn.OpenAsync();

            var tickers = new System.Collections.Generic.List<string>();
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = "SELECT ticker FROM watchlist ORDER BY ticker LIMIT 10;";
                using var rdr = await cmd.ExecuteReaderAsync();
                while (await rdr.ReadAsync())
                    tickers.Add(rdr.GetString(0));
            }

            int alertCount;
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM alerts WHERE enabled=1;";
                alertCount = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            }

            var sb = new StringBuilder();
            sb.AppendLine($"Trader Forge — Daily Brief ({DateTime.Today:yyyy-MM-dd})");
            sb.AppendLine();
            sb.AppendLine($"Watchlist: {(tickers.Count == 0 ? "—" : string.Join(\", \", tickers))}");
            sb.AppendLine($"Active alerts: {alertCount}");

            try
            {
                if (tickers.Count > 0)
                {
                    var prices = await _prices.GetPricesAsync(tickers, default);
                    var first = prices.FirstOrDefault();
                    if (first.ticker != null)
                        sb.AppendLine($"Sample price — {first.ticker}: {first.price:0.00}");
                }
            }
            catch { /* CI/offline safe */ }

            sb.AppendLine();
            sb.AppendLine("Quote: “Stay disciplined. Small edges compound.”");
            return sb.ToString();
        }
    }
}
