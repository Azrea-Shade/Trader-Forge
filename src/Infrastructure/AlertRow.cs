namespace Infrastructure
{
    // Basic alert row model stored in SQLite table 'alerts'
    public record AlertRow(long Id, string Ticker, double? Above, double? Below, bool Enabled);
}
