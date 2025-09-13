using Services.Engines;
using System.Collections.Generic;

namespace Presentation
{
    public static class AlertEngineShim
    {
        public static IEnumerable<Services.Engines.AlertResult> Evaluate(object a, object b)
            => new Services.Engines.AlertEngine().Evaluate(a, b);

        public static IEnumerable<(Services.Engines.AlertResult alert, double? price)> EvaluateWithPrices(object watchlist, object prices)
            => new Services.Engines.AlertEngine().EvaluateWithPrices(watchlist, prices);
    }
}
