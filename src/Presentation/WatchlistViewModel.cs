using System.Collections.Generic;
using Domain;

namespace Presentation
{
    /// <summary>
    /// Thin proxy to the static Domain.AlertEngine. No instance fields.
    /// </summary>
    public class WatchlistViewModel
    {
        public IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow)> Evaluate(object a, object b)
            => AlertEngine.Evaluate(a, b);

        public IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow, double? Price)> EvaluateWithPrices(object watchlist, object prices)
            => AlertEngine.EvaluateWithPrices(watchlist, prices);
    }
}
