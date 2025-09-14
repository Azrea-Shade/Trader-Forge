using System.Collections.Generic;
using System.Linq;
using Services.Engines;

namespace Presentation
{
    /// <summary>Adapt Services.Engines.AlertEngine to tuple shapes the tests expect.</summary>
    public static class AlertEngineShim
    {
        public static IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow)>
            Evaluate(object a, object b) =>
            new AlertEngine()
                .Evaluate(a, b)
                .Select(r => (r.Id, r.TriggeredAbove, r.TriggeredBelow));

        public static IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow, double? Price)>
            EvaluateWithPrices(object watchlist, object prices) =>
            new AlertEngine()
                .EvaluateWithPrices(watchlist, prices)
                .Select(x => (x.alert.Id, x.alert.TriggeredAbove, x.alert.TriggeredBelow, x.price));
    }
}
