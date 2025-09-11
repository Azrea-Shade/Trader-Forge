using System;

namespace Services
{
    // Minimal stub to satisfy compilation; flesh out later to meet behavior.
    public class PortfolioService
    {
        private readonly object _a;
        private readonly object _b;

        public PortfolioService(object a, object b)
        {
            _a = a;
            _b = b;
        }

        public override string ToString() => "PortfolioService(stub)";
    }
}
