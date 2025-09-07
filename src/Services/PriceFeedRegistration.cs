using System;
using Microsoft.Extensions.DependencyInjection;

namespace Services
{
    public static class PriceFeedRegistration
    {
        // Default "dummy" keeps CI safe; set PRICEFEED_MODE=live locally for Yahoo/Stooq.
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
