using System.Collections.Generic;
using System.Linq;
using Services.Engines; // concrete engine + AlertResult

namespace Domain
{
    // Static facade so tests can call AlertEngine.Evaluate(...) without an instance.
    public static class AlertEngine
    {
        private static readonly Services.Engines.AlertEngine _engine = new();

        public static IEnumerable<Services.Engines.AlertResult>
            Evaluate(object watchlist, object prices)
            => _engine.Evaluate(watchlist, prices);

        public static IEnumerable<(Services.Engines.AlertResult alert, double? price)>
            EvaluateWithPrices(object watchlist, object prices)
            => _engine.EvaluateWithPrices(watchlist, prices);

        // Flatten to the exact tuple the Phase4 test asserts on; Price is double? for .HasValue
        public static IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow, double? Price)>
            EvaluateWithPricesFlattened(object watchlist, object prices)
            => _engine.EvaluateWithPrices(watchlist, prices)
                     .Select(x => (x.alert.Id, x.alert.TriggeredAbove, x.alert.TriggeredBelow, x.price));
    }
}
