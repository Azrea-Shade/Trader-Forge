using System;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    /// <summary>
    /// Contract for the Daily Brief feature.
    /// Keep this file free of concrete service implementations (those live elsewhere).
    /// </summary>
    public interface IBriefingService
    {
        /// <summary>
        /// Generate the daily brief text (already formatted for display).
        /// </summary>
        Task<string> GenerateAsync(CancellationToken ct = default);
    }

    /// <summary>
    /// Backward-compatible model so existing call sites compile:
    /// - Parameterless construction supported via default-inited properties
    /// - Properties expected by UI/Services: GeneratedAtLocal, SummaryText, Tickers, Sections, Date
    /// </summary>
    public sealed record class DailyBrief
    {
        public DateOnly Date { get; init; } =
            DateOnly.FromDateTime(DateTime.Now);

        public BriefSection[] Sections { get; init; } =
            Array.Empty<BriefSection>();

        /// <summary>Local timestamp the brief was generated.</summary>
        public DateTimeOffset GeneratedAtLocal { get; init; } =
            DateTimeOffset.Now;

        /// <summary>One-line summary used by scheduler/toast/UI.</summary>
        public string SummaryText { get; init; } = string.Empty;

        /// <summary>Tickers referenced in this brief (watchlist snapshot, movers, etc.).</summary>
        public string[] Tickers { get; init; } = Array.Empty<string>();
    }

    /// <summary>
    /// Simple section block for a brief.
    /// </summary>
    public sealed record BriefSection(string Title, string Body);
}
