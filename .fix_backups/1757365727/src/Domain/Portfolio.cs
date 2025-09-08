// Phase 5: Domain models
namespace Domain
{
    public class Portfolio
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public string? Notes { get; set; }
    }

    public class Holding
    {
        public long Id { get; set; }
        public long PortfolioId { get; set; }
        public string Ticker { get; set; } = "";
        public double Shares { get; set; }
        public double CostBasis { get; set; } // total cost for these shares
    }

    public class AllocationRow
    {
        public string Ticker { get; set; } = "";
        public double MarketValue { get; set; }
        public double WeightPct { get; set; } // 0..100
    }

    public class PortfolioSummary
    {
        public long PortfolioId { get; set; }
        public string Name { get; set; } = "";
        public double TotalCost { get; set; }
        public double TotalMarketValue { get; set; }
        public double GainLoss { get; set; }
        public double GainLossPct { get; set; } // -100..+∞
        public List<AllocationRow> Allocation { get; set; } = new();
    }
}
