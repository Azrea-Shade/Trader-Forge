using System.Collections.Generic;
using Services;
using System;
using System.Linq;
using System.Text;
using Domain;
using Infrastructure;

namespace Services
{
    public interface INotifier
    {
        void Notify(string title, string message);
    }

    // Simple file-based notifier (CI/Termux safe); swap for Windows toast later.
    public sealed class FileNotifier : INotifier
    {
        private readonly string _path;
        public FileNotifier(string? path = null)
        {
            var baseDir = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AzreaCompanion");
            System.IO.Directory.CreateDirectory(baseDir);
            _path = path ?? System.IO.Path.Combine(baseDir, "notifications.log");
        }

        public void Notify(string title, string message)
        {
            var line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {title}: {message}{Environment.NewLine}";
            System.IO.File.AppendAllText(_path, line);
        }
    }

    public class BriefService
    {
        private readonly WatchlistReader _watch;
        private readonly PortfolioService _portfolios;
        private readonly IClock _clock;

        public BriefService(WatchlistReader watch, PortfolioService portfolios, IClock clock)
        {
            _watch = watch;
            _portfolios = portfolios;
            _clock = clock;
        }

        public DailyBrief Generate(DateOnly date, string[] tickers)
        {
            var tickers = _watch.AllTickers();
            var sb = new StringBuilder();
            sb.AppendLine($"Good morning — {_clock.Now:dddd, MMM d}.");
            sb.AppendLine($"Watchlist: {(tickers.Count == 0 ? "empty" : string.Join(", ", tickers))}.");

            // Try summarizing first portfolio if any
            var p = _portfolios.AllPortfolios().FirstOrDefault();
            if (p != null)
            {
                var s = _portfolios.Summary(p.Id);
                sb.AppendLine($"Portfolio “{p.Name}”: MV={s.TotalMarketValue:C} P/L={s.GainLoss:C} ({s.GainLossPct:F2}%).");
                if (s.Allocation.Count > 0)
                {
                    var top = s.Allocation.OrderByDescending(a => a.WeightPct).Take(3).ToList();
                    sb.Append("Top weights: ");
                    sb.AppendLine(string.Join(", ", top.Select(a => $"{a.Ticker} {a.WeightPct:F1}%")));
                }
            }
            else
            {
                sb.AppendLine("No portfolios yet.");
            }

            return new DailyBrief
            {
                GeneratedAtLocal = _clock.Now,
                SummaryText = sb.ToString(),
                Tickers = tickersToArray()
            };
        }
    }
}
