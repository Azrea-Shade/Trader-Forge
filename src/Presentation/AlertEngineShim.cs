using System.Collections.Generic;
using Services.Engines;

namespace Presentation
{
    public static class AlertEngineShim
    {
        public static IEnumerable<AlertResult> Evaluate(object a, object b)
            => new AlertEngine().Evaluate(a, b);

        public static IEnumerable<(AlertResult alert, double? price)> EvaluateWithPrices(object watchlist, object prices)
            => new AlertEngine().EvaluateWithPrices(watchlist, prices);
    }
}
