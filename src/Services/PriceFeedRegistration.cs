using System;
using Microsoft.Extensions.DependencyInjection;

namespace Services
{
    public static class PriceFeedRegistration
    {
        // CI safety: default to "dummy". To enable live locally: export PRICEFEED_MODE=live
        public static IServiceCollection AddTraderForgePriceFeed(this IServiceCollection services)
        {
            var mode = Environment.GetEnvironmentVariable("PRICEFEED_MODE") ?? "dummy";
            if (string.Equals(mode, "live", StringComparison.OrdinalIgnoreCase))
            {
                services.AddSingleton<IPriceFeed, LivePriceAdapter>();
            }
            else
            {
                services.AddSingleton<IPriceFeed, DummyPriceFeed>();
            }
            return services;
        }
    }
}
