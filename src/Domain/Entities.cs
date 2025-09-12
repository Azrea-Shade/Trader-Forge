namespace Domain
{
    public record Symbol(string Ticker, string Name);
    public record WatchItem(string Ticker, decimal? AlertAbove, decimal? AlertBelow);
}
