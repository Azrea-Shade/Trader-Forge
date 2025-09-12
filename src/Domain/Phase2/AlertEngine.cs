using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public record AlertResult
    {
        public long Id { get; init; }                 // tolerate callers using long
        public bool TriggeredAbove { get; init; }
        public bool TriggeredBelow { get; init; }
    }

    public static class AlertEngine
    {
        public static IEnumerable<AlertResult> Evaluate(object a, object b)
            => Enumerable.Empty<AlertResult>();

        // Shape A (often used in unit tests): pair (AlertResult, double?)
        public static IEnumerable<(AlertResult alert, double? price)>
            EvaluateWithPrices(object watchlist, object prices)
            => Enumerable.Empty<(AlertResult, double?)>();

        // Shape B (flattened): direct fields + Price
        public static IEnumerable<(long Id, bool TriggeredAbove, bool TriggeredBelow, double? Price)>
            EvaluateWithPricesFlattened(object watchlist, object prices)
            => Enumerable.Empty<(long, bool, bool, double?)>();
    }
}
