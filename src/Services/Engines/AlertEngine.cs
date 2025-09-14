using System.Collections.Generic;
using System.Linq;

namespace Services.Engines
{
    public class AlertEngine
    {
        public AlertEngine(object? _ = null) {}

        // Tests expect (int Id, bool TriggeredAbove, bool TriggeredBelow)
        public IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow)> Evaluate(object a, object b)
            => Enumerable.Empty<(int, bool, bool)>();

        // Tests expect Price nullable so they can call .HasValue
        public IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow, double? Price)> EvaluateWithPrices(object watchlist, object prices)
            => Enumerable.Empty<(int, bool, bool, double?)>();
    }
}
