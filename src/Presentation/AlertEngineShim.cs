using System.Collections.Generic;
using Services.Engines;

namespace Presentation
{
    /// Forwards Services.Engines.AlertEngine results as flattened tuples
    /// (Id, TriggeredAbove, TriggeredBelow[, Price]) that Phase4 tests expect.
    public static class AlertEngineShim
    {
        public static IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow)>
            Evaluate(object a, object b)
            => new AlertEngine().Evaluate(a, b);

        public static IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow, double? Price)>
            EvaluateWithPrices(object watchlist, object prices)
            => new AlertEngine().EvaluateWithPrices(watchlist, prices);
    }
}
