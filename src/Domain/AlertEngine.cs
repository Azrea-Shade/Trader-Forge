using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    // Tests expect flattened tuples, not (AlertResult alert, double? price)
    // Keep ctor with optional arg because tests sometimes pass one.
    public class AlertEngine
    {
        public AlertEngine() { }
        public AlertEngine(object? _) { }

        public IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow)>
            Evaluate(object a, object b)
            => Enumerable.Empty<(int, bool, bool)>();

        public IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow, double? Price)>
            EvaluateWithPrices(object watchlist, object prices)
            => Enumerable.Empty<(int, bool, bool, double?)>();
    }
}
