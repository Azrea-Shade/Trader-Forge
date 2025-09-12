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

        // Return (alert, price) where price is double; DoubleExtensions.HasValue() handles null-like semantics (NaN)
        public static IEnumerable<(AlertResult alert, double price)> EvaluateWithPrices(object watchlist, object prices)
            => Enumerable.Empty<(AlertResult, double)>();
    }
}
