using System.Collections.Generic;

namespace Domain
{
    /// <summary>Summary of a portfolio with its allocation rows.</summary>
    public sealed record PortfolioSummary(int PortfolioId, string Name, List<AllocationRow> Allocation);
}
