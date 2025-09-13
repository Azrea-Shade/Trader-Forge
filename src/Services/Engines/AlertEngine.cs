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

    public class AlertEngine
    {
        public AlertEngine(object? context = null) { }

        public IEnumerable<AlertResult> Evaluate(object a, object b)
            => Enumerable.Empty<AlertResult>();

        // FLATTENED named tuple: tests access .Id/.TriggeredAbove/.TriggeredBelow and .Price.HasValue
        public IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow, double? Price)>
            EvaluateWithPrices(object watchlist, object prices)
            => Enumerable.Empty<(int, bool, bool, double?)>();
    }
}
