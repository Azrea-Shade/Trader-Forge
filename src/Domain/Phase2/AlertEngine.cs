using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public record AlertResult
    {
        public int Id { get; init; }
        public double? TriggeredAbove { get; init; }
        public double? TriggeredBelow { get; init; }
    }

    public static class AlertEngine
    {
        public static IEnumerable<AlertResult> Evaluate(object a, object b)
            => Enumerable.Empty<AlertResult>();

        public static IEnumerable<AlertResult> EvaluateWithPrices(object watchlist, object prices)
            => Enumerable.Empty<AlertResult>();
    }
}
