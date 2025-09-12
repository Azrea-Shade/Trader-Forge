using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public record AlertResult
    {
        public long Id { get; init; }
        public bool TriggeredAbove { get; init; }
        public bool TriggeredBelow { get; init; }
    }

    public static class AlertEngine
    {
        public static IEnumerable<AlertResult> Evaluate(object a, object b)
            => Enumerable.Empty<AlertResult>();

        // Tests expect flattened fields + nullable Price (double?)
        public static IEnumerable<(long Id, bool TriggeredAbove, bool TriggeredBelow, double? Price)>
            EvaluateWithPrices(object watchlist, object prices)
            => Enumerable.Empty<(long, bool, bool, double?)>();

        // Optional pair form if any caller wants the AlertResult back
        public static IEnumerable<(AlertResult alert, double? price)>
            EvaluateWithPairs(object watchlist, object prices)
            => Enumerable.Empty<(AlertResult, double?)>();
    }
}
