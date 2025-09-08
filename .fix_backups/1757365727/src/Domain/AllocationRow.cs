namespace Domain
{
    /// <summary>Single ticker weight row used in portfolio allocation displays.</summary>
    public sealed record AllocationRow(string Ticker, decimal Weight);
}
