using System.Collections.Generic;
using Domain;

namespace Services.Engines
{
    /// <summary>
    /// Thin wrapper so older tests that new-up an engine keep working.
    /// Methods forward to static Domain.AlertEngine and keep tuple shapes,
    /// with Price guaranteed as nullable double?.
    /// </summary>
    public class AlertEngine
    {
        // Tests sometimes call new AlertEngine() or new AlertEngine(anything)
        public AlertEngine() {}
        public AlertEngine(object _) {}

        public IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow)>
            Evaluate(object a, object b) =>
            Domain.AlertEngine.Evaluate(a, b);

        public IEnumerable<(int Id, bool TriggeredAbove, bool TriggeredBelow, double? Price)>
            EvaluateWithPrices(object watchlist, object prices) =>
            Domain.AlertEngine.EvaluateWithPrices(watchlist, prices);
    }
}
