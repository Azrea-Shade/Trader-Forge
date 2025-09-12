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
        public static IEnumerable<AlertResult> Evaluate(object a, object b)
            => Enumerable.Empty<AlertResult>();

        // Return alerts and nullable prices (so .HasValue is valid on double?)
        public static (IEnumerable<AlertResult> alerts, IEnumerable<double?> prices)
            EvaluateWithPrices(object watchlist, object prices)
            => (Enumerable.Empty<AlertResult>(), Enumerable.Empty<double?>());
    }
}
