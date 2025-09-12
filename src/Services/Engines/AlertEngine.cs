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
        public static IEnumerable<AlertResult> Evaluate(object a, object b)
            => Enumerable.Empty<AlertResult>();

        // Flattened tuple; nullable price so tests can use .price.HasValue
        public static IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow, double? price)>
            EvaluateWithPrices(object watchlist, object prices)
            => Enumerable.Empty<(int, bool, bool, double?)>();
    }
}
