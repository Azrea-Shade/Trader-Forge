namespace Domain.Entities
{
    public sealed class Holding
    {
        public string Ticker { get; init; } = "";
        public decimal Quantity { get; init; }
        public decimal AverageCost { get; init; }
    }
}
