namespace Services.Engines
{
    public record AlertRow(long Id, string Ticker, double? Above, double? Below);

    public class AlertEvaluation
    {
        public long   Id { get; set; }
        public string Ticker { get; set; } = "";
        public double Price { get; set; }
        public double? Above { get; set; }
        public double? Below { get; set; }
        public bool   TriggeredAbove { get; set; }
        public bool   TriggeredBelow { get; set; }
    }
}
