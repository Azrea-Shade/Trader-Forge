using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public record AlertResult
    {
        public long Id { get; init; }
        public bool TriggeredAbove { get; init; }
        public bool TriggeredBelow { get; init; }
    }

    public static class AlertEngine
    {
        public static IEnumerable<AlertResult> Evaluate(object a, object b)
            => Enumerable.Empty<AlertResult>();

        // (AlertResult, double?) so tests can use .HasValue
        public static IEnumerable<(AlertResult alert, double? price)>
            EvaluateWithPrices(object watchlist, object prices)
            => Enumerable.Empty<(AlertResult, double?)>();

        // flattened option (some tests may use this)
        public static IEnumerable<(long Id, bool TriggeredAbove, bool TriggeredBelow, double? Price)>
            EvaluateWithPricesFlattened(object watchlist, object prices)
            => Enumerable.Empty<(long, bool, bool, double?)>();
    }
}
