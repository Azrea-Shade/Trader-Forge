using System.Collections.Generic;
using Domain;

namespace Presentation
{
    /// <summary>Flat proxies to static Domain.AlertEngine.</summary>
    public static class AlertEngineShim
    {
        public static IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow)>
            Evaluate(object a, object b) => AlertEngine.Evaluate(a, b);

        public static IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow, double? Price)>
            EvaluateWithPrices(object watchlist, object prices) =>
            AlertEngine.EvaluateWithPrices(watchlist, prices);
    }
}
