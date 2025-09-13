using System.Collections.Generic;
using System.Linq;

namespace Services.Engines
{
    // A minimal shape-only result so tests compile; behavior can be filled in later.
    public record AlertResult(int Id, bool TriggeredAbove, bool TriggeredBelow);

    public class AlertEngine
    {
        public AlertEngine(object? _ = null) {}

        // Phase2/3 callers may still use this
        public IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow)> Evaluate(object a, object b)
            => Enumerable.Empty<(int, bool, bool)>();

        // Phase4 expects flattened tuple with Price as double?
        public IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow, double? Price)> EvaluateWithPrices(object watchlist, object prices)
            => Enumerable.Empty<(int, bool, bool, double?)>();
    }
}
