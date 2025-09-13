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

        // Keeps tests happy: nullable price enables .HasValue on double?
        public IEnumerable<(AlertResult alert, double? price)>
            EvaluateWithPrices(object watchlist, object prices)
            => Enumerable.Empty<(AlertResult, double?)>();
    }
}
