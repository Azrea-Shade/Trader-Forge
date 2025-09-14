using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    /// <summary>
    /// Compile-shape implementation for tests:
    ///   Evaluate(a,b) -> IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow)>
    ///   EvaluateWithPrices(w,p) -> IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow, double? Price)>
    /// Also exposes 0/1-arg ctors because ServiceFactory calls the 1-arg form.
    /// NOTE: Minimal bodies to satisfy the compiler; real logic lives elsewhere.
    /// </summary>
    public class AlertEngine
    {
        public AlertEngine() { }
        public AlertEngine(object? _) { }

        public IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow)>
            Evaluate(object a, object b)
            => Enumerable.Empty<(int, bool, bool)>();

        public IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow, double? Price)>
            EvaluateWithPrices(object watchlist, object prices)
            => Enumerable.Empty<(int, bool, bool, double?)>();
    }
}
