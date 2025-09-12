using System.Collections.Generic;
using System.Linq;

namespace Services.Engines
{
    public record AlertResult
    {
        public int Id { get; init; }
        public bool TriggeredAbove { get; init; }
        public bool TriggeredBelow { get; init; }
    }

    public static class AlertEngine
    {
        // Placeholder logic; tests only check shape/compile right now.
        public static IEnumerable<AlertResult> Evaluate(object a, object b)
            => Enumerable.Empty<AlertResult>();

        // MUST match tests: pair of (AlertResult alert, double? price)
        public static IEnumerable<(AlertResult alert, double? price)> EvaluateWithPrices(object watchlist, object prices)
            => Enumerable.Empty<(AlertResult, double?)>();
    }
}
