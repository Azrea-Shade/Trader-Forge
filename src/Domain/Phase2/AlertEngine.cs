using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public static class AlertEngine
    {
        // Enumerable so tests can call .ToList() etc.
        public static IEnumerable<object> Evaluate(object a, object b)
            => Enumerable.Empty<object>();
    }
}
