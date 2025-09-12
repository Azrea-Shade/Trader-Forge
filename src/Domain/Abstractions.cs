namespace Domain
{
    public interface IQuoteProvider
    {
        decimal? TryGetPrice(string ticker);
    }
}
