using System.Collections.Generic;
using Services.Engines;

namespace Presentation
{
    public static class AlertEngineShim
    {
        public static IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow)>
            Evaluate(object a, object b) =>
            new AlertEngine().Evaluate(a, b);

        public static IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow, double? Price)>
            EvaluateWithPrices(object watchlist, object prices) =>
            new AlertEngine().EvaluateWithPrices(watchlist, prices);
    }
}
