using System.Collections.Generic;
using System.Linq;

namespace Services.Engines
{
    // Tests expect FLAT tuples (Id, TriggeredAbove, TriggeredBelow[, Price])
    public class AlertEngine
    {
        public AlertEngine(object? _ = null) {}

        public IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow)> Evaluate(object a, object b)
            => Enumerable.Empty<(int, bool, bool)>();

        public IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow, double? Price)> EvaluateWithPrices(object watchlist, object prices)
            => Enumerable.Empty<(int, bool, bool, double?)>();
    }
}
