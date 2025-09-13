using Services.Engines;
using System.Collections.Generic;
using Domain;

namespace Presentation
{
    public class AlertEngine
    {
        public AlertEngine(object? _ = null) { }
        public IEnumerable<AlertResult> Evaluate(object a, object b)
            => AlertEngine.Evaluate(a, b);
    }
}
