using System;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace Services
{
    public static class PriceFeedRegistration
    {
        // Default mode is "dummy" to keep CI safe unless explicitly set to "live"
        public static IServiceCollection AddTraderForgePriceFeed(this IServiceCollection services)
        {
            var mode = Environment.GetEnvironmentVariable("PRICEFEED_MODE") ?? "dummy";
            if (string.Equals(mode, "live", StringComparison.OrdinalIgnoreCase))
            {
                services.AddHttpClient();
                services.AddSingleton<IPriceFeed, LivePriceRouter>();
            }
            else
            {
                services.AddSingleton<IPriceFeed, DummyPriceFeed>();
            }
            return services;
        }
    }
}
