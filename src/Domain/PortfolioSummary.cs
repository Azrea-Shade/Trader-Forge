using System.Collections.Generic;

namespace Domain
{
    // Canonical PortfolioSummary with safe initialization
    public record PortfolioSummary
    {
        public int PortfolioId { get; init; }
        public string Name { get; init; } = string.Empty;
        public List<AllocationRow> Allocation { get; init; } = new();

        public PortfolioSummary() { }

        public PortfolioSummary(int portfolioId, string name, List<AllocationRow>? allocation = null)
        {
            PortfolioId = portfolioId;
            Name = name ?? string.Empty;
            Allocation = allocation ?? new List<AllocationRow>();
        }
    }
}
