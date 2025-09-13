using AlertResultSvc = Services.Engines.AlertResult;
using AlertEngineSvc = Services.Engines.AlertEngine;
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
