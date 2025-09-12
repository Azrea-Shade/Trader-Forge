using System.Collections.Generic;

namespace Services.Engines
{
    public partial class AlertEngine
    {
        private readonly Services.Feeds.IPriceFeed? _priceFeed;
        public AlertEngine(Services.Feeds.IPriceFeed? priceFeed) => _priceFeed = priceFeed;
        // Other members live in partials.
    }
}
