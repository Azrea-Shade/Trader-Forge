using System.Collections.Generic;
using System.Linq;

namespace Services.Engines
{
    // Tests and Presentation disagree on shape; provide both.
    public record AlertResult(int Id, bool TriggeredAbove, bool TriggeredBelow);

    public class AlertEngine
    {
        public AlertEngine(object? _ = null) {}

        // Shape A: IEnumerable<AlertResult>
        public IEnumerable<AlertResult> Evaluate(object a, object b)
            => Enumerable.Empty<AlertResult>();

        // Shape B: IEnumerable<(AlertResult alert, double? price)>
        public IEnumerable<(AlertResult alert, double? price)> EvaluateWithPrices(object watchlist, object prices)
            => Evaluate(watchlist, prices).Select(ar => (ar, (double?)null));

        // Shape C (flattened): IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow, double? price)>
        public IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow, double? price)>
            EvaluateWithPricesFlattened(object watchlist, object prices)
            => EvaluateWithPrices(watchlist, prices)
                .Select(t => (t.alert.Id, t.alert.TriggeredAbove, t.alert.TriggeredBelow, t.price));
    }
}
