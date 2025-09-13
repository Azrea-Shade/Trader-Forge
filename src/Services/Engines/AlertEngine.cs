using System.Collections.Generic;
using System.Linq;

namespace Services.Engines
{
    public record AlertResult(int Id, bool TriggeredAbove, bool TriggeredBelow);

    public class AlertEngine
    {
        public AlertEngine(object? _ = null) {}

        public IEnumerable<AlertResult> Evaluate(object a, object b)
            => Enumerable.Empty<AlertResult>();

        public IEnumerable<(AlertResult alert, double? price)> EvaluateWithPrices(object watchlist, object prices)
            => Enumerable.Empty<(AlertResult, double?)>();
    }
}
