using System.Collections.Generic;
using System.Linq;
using Services.Engines;

namespace Presentation
{
    public static class AlertEngineShim
    {
        // Tests expect: IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow)>
        public static IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow)>
            Evaluate(object a, object b)
            => new AlertEngine().Evaluate(a, b)
               .Select(ar => (ar.Id, ar.TriggeredAbove, ar.TriggeredBelow));

        // Tests expect: IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow, double? Price)>
        public static IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow, double? Price)>
            EvaluateWithPrices(object watchlist, object prices)
            => new AlertEngine().EvaluateWithPrices(watchlist, prices)
               .Select(t => (t.alert.Id, t.alert.TriggeredAbove, t.alert.TriggeredBelow, t.price));
    }
}
