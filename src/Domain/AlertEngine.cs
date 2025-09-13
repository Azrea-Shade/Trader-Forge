using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public record AlertResult
    {
        public int Id { get; init; }
        public bool TriggeredAbove { get; init; }
        public bool TriggeredBelow { get; init; }
    }

    public static class AlertEngine
    {
        private static Services.Engines.AlertEngine Impl() => new Services.Engines.AlertEngine();

        public static IEnumerable<AlertResult> Evaluate(object a, object b)
            => Impl().Evaluate(a, b).Select(x => new AlertResult { Id = x.Id, TriggeredAbove = x.TriggeredAbove, TriggeredBelow = x.TriggeredBelow });

        public static IEnumerable<(AlertResult alert, double? price)> EvaluateWithPrices(object watchlist, object prices)
            => Impl().EvaluateWithPrices(watchlist, prices)
                     .Select(x => (new AlertResult { Id = x.alert.Id, TriggeredAbove = x.alert.TriggeredAbove, TriggeredBelow = x.alert.TriggeredBelow }, x.price));

        public static IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow, double? Price)> EvaluateWithPricesFlattened(object watchlist, object prices)
            => EvaluateWithPrices(watchlist, prices).Select(x => (x.alert.Id, x.alert.TriggeredAbove, x.alert.TriggeredBelow, x.price));
    }
}
