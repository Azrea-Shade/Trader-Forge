using System;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    // Contract for the Daily Brief feature. Keep this file free of concrete implementations.
    public interface IBriefingService
    {
        /// <summary>
        /// Generate the daily brief text (already formatted for display).
        /// </summary>
        Task<string> GenerateAsync(CancellationToken ct = default);
    }

    // (Optional) lightweight model types if/when you expand the brief
    public sealed record BriefSection(string Title, string Body);
    public sealed record DailyBrief(DateOnly Date, BriefSection[] Sections);
}
