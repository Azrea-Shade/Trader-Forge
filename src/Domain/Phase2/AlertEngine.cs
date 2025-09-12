using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public record AlertResult
    {
        public long Id { get; init; }                 // long to tolerate callers passing long
        public bool TriggeredAbove { get; init; }
        public bool TriggeredBelow { get; init; }
    }

    public static class AlertEngine
    {
        public static IEnumerable<AlertResult> Evaluate(object a, object b)
            => Enumerable.Empty<AlertResult>();

        // Flattened tuple so tests can access fields directly; Price is double? so .HasValue works
        public static IEnumerable<(long Id, bool TriggeredAbove, bool TriggeredBelow, double? Price)>
            EvaluateWithPrices(object watchlist, object prices)
            => Enumerable.Empty<(long, bool, bool, double?)>();
    }
}
