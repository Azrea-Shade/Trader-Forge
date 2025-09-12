using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public record AlertResult
    {
        public string Id { get; init; } = string.Empty;
        public bool TriggeredAbove { get; init; }
        public bool TriggeredBelow { get; init; }
    }

    public static class AlertEngine
    {
        public static IEnumerable<AlertResult> Evaluate(object a, object b)
            => Enumerable.Empty<AlertResult>();
    }
}
