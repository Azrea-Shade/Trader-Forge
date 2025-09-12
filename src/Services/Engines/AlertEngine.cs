using System.Collections.Generic;
using Domain;

namespace Services.Engines
{
    public class AlertEngine
    {
        public AlertEngine(object? _ = null) { }

        public IEnumerable<AlertResult> Evaluate(object a, object b)
            => Domain.AlertEngine.Evaluate(a, b);

        // Match flattened signature expected by tests
        public IEnumerable<(long Id, bool TriggeredAbove, bool TriggeredBelow, double? Price)>
            EvaluateWithPrices(object watchlist, object prices)
            => Domain.AlertEngine.EvaluateWithPrices(watchlist, prices);

        // Convenience passthrough for the pair variant
        public IEnumerable<(AlertResult alert, double? price)>
            EvaluateWithPairs(object watchlist, object prices)
            => Domain.AlertEngine.EvaluateWithPairs(watchlist, prices);
    }
}
