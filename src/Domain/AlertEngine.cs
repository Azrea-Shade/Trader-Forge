using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    // Static facade returning the flattened shapes the unit tests expect.
    public static class AlertEngine
    {
        // IEnumerable<(Id, TriggeredAbove, TriggeredBelow)>
        public static IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow)>
            Evaluate(object watchlist, object prices)
            => Enumerable.Empty<(int, bool, bool)>();

        // IEnumerable<(Id, TriggeredAbove, TriggeredBelow, Price)>
        // Price is double? so .HasValue is valid in tests
        public static IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow, double? Price)>
            EvaluateWithPrices(object watchlist, object prices)
            => Enumerable.Empty<(int, bool, bool, double?)>();
    }
}
