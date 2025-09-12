using System.Collections.Generic;
using Domain;

namespace Services.Engines
{
    public class AlertEngine
    {
        public AlertEngine(object _ = null) { }

        // Instance forwards
        public IEnumerable<AlertResult> Evaluate(object a, object b)
            => Domain.AlertEngine.Evaluate(a, b);

        public IEnumerable<(AlertResult alert, double? price)> EvaluateWithPrices(object watchlist, object prices)
            => Domain.AlertEngine.EvaluateWithPrices(watchlist, prices);

        public IEnumerable<(long Id, bool TriggeredAbove, bool TriggeredBelow, double? Price)> EvaluateWithPricesFlattened(object watchlist, object prices)
            => Domain.AlertEngine.EvaluateWithPricesFlattened(watchlist, prices);

        // Static convenience (some sites call AlertEngine.Evaluate(...))
        public static IEnumerable<AlertResult> Evaluate(object a, object b)
            => Domain.AlertEngine.Evaluate(a, b);

        public static IEnumerable<(AlertResult alert, double? price)> EvaluateWithPrices(object watchlist, object prices)
            => Domain.AlertEngine.EvaluateWithPrices(watchlist, prices);

        public static IEnumerable<(long Id, bool TriggeredAbove, bool TriggeredBelow, double? Price)> EvaluateWithPricesFlattened(object watchlist, object prices)
            => Domain.AlertEngine.EvaluateWithPricesFlattened(watchlist, prices);
    }
}
