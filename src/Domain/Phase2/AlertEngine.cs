namespace Domain
{
using System.Collections.Generic;
using System.Linq;

public record AlertResult
{
    public int Id { get; init; }
    public bool TriggeredAbove { get; init; }
    public bool TriggeredBelow { get; init; }
}

public static class AlertEngine
{
    public static IEnumerable<AlertResult> Evaluate(object a, object b)
        => Enumerable.Empty<AlertResult>();

    // Flattened tuple so tests can do x.Id / x.TriggeredAbove / x.Price.HasValue
    public static IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow, double? Price)>
        EvaluateWithPrices(object watchlist, object prices)
        => Enumerable.Empty<(int, bool, bool, double?)>();
}
}
