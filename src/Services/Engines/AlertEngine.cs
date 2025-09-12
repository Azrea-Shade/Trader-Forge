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

    // Non-static class so callers can 'new AlertEngine()'
    public class AlertEngine
    {
        // Static APIs (for tests or static callers)
        public static IEnumerable<AlertResult> Evaluate(object a, object b)
            => Enumerable.Empty<AlertResult>();

        public static IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow, double? price)>
            EvaluateWithPrices(object watchlist, object prices)
            => Enumerable.Empty<(int, bool, bool, double?)>();

        // Instance wrappers (for Presentation/WatchlistViewModel)
        public IEnumerable<AlertResult> EvaluateInstance(object a, object b)
            => Evaluate(a, b);

        public IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow, double? price)>
            EvaluateWithPricesInstance(object watchlist, object prices)
            => EvaluateWithPrices(watchlist, prices);
    }
}
