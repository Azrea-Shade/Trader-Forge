using Domain;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    /// <summary>
    /// Minimal, CI-safe implementation of IBriefingService.
    /// You can enrich this later with real data + formatting.
    /// </summary>
    public sealed class BriefingService : IBriefingService
    {
        public Task<string> GenerateAsync(CancellationToken ct = default)
        {
            var now = DateTimeOffset.Now;
            var sb = new StringBuilder();
            sb.AppendLine($"Trader Forge – Daily Brief");
            sb.AppendLine($"Date: {now:yyyy-MM-dd} (local {now:HH:mm})");
            sb.AppendLine();
            sb.AppendLine("• Quote: Stay focused. Small, steady gains compound.");
            sb.AppendLine("• Tip: Review your watchlist alerts before market open.");
            return Task.FromResult(sb.ToString());
        }
    }
}
