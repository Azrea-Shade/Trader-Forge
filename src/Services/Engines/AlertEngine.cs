using System.Collections.Generic;
using System.Linq;

namespace Services.Engines
{
    // Result shape the tests expect to project from
    public record AlertResult
    {
        public int Id { get; init; }
        public bool TriggeredAbove { get; init; }
        public bool TriggeredBelow { get; init; }
    }

    // Instance engine; real impl can replace the stubs below
    public class AlertEngine
    {
        public AlertEngine() { }

        public IEnumerable<AlertResult> Evaluate(object watchlist, object prices)
            => Enumerable.Empty<AlertResult>();

        // Keep price nullable so tests can use .HasValue
        public IEnumerable<(AlertResult alert, double? price)> EvaluateWithPrices(object watchlist, object prices)
            => Enumerable.Empty<(AlertResult, double?)>();
    }
}
