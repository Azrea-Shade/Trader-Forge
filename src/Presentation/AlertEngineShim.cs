using System.Collections.Generic;
using Domain;

namespace Presentation
{
    // Simple shim to satisfy instance-based usage in WatchlistViewModel
    public class AlertEngine
    {
        public AlertEngine(object _ = null) { }
        public IEnumerable<AlertResult> Evaluate(object a, object b)
            => Domain.AlertEngine.Evaluate(a, b);
    }
}
