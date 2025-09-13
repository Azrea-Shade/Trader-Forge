using AlertEngineAlias = Services.Engines.AlertEngineAlias;
using AlertResultAlias = Services.Engines.AlertResultAlias;
using Services.Engines;

using AlertResultSvc = Services.Engines.AlertResultAlias;
using AlertEngineSvc = Services.Engines.AlertEngineAlias;
using Services.Engines;
using System.Collections.Generic;
using Domain;

namespace Presentation
{
    public class AlertEngineSvc
    {
        public AlertEngineSvc(object? _ = null) { }
        public IEnumerable<AlertResultSvc> Evaluate(object a, object b)
            => (new AlertEngineSvc()).Evaluate(a, b);
    }
}
