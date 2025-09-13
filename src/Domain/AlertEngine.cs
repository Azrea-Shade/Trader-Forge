using System.Collections.Generic;
using System.Linq;

namespace Domain
{
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

        // Tests expect flattened tuple with Price as double? (for .HasValue)
        public static IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow, double? Price)>
            EvaluateWithPrices(object watchlist, object prices)
            => Enumerable.Empty<(int, bool, bool, double?)>();

        // Alias retained for callers that use the *Flattened* name
        public static IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow, double? Price)>
            EvaluateWithPricesFlattened(object watchlist, object prices)
            => EvaluateWithPrices(watchlist, prices);
    }
}
