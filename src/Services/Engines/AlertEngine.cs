using System.Collections.Generic;
using System.Linq;

namespace Services.Engines
{
    // Kept for compatibility if referenced elsewhere.
    public record AlertResult
    {
        public int Id { get; init; }
        public bool TriggeredAbove { get; init; }
        public bool TriggeredBelow { get; init; }
    }

    public class AlertEngine
    {
        public IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow)>
            Evaluate(object a, object b) =>
            Enumerable.Empty<(int, bool, bool)>();

        public IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow, double? Price)>
            EvaluateWithPrices(object watchlist, object prices) =>
            Enumerable.Empty<(int, bool, bool, double?)>();
    }
}
