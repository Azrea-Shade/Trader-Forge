using System;
using System.Linq;
using System.Reflection;
using Infrastructure;
using Services;
using Services.Feeds;
using FeedAbstraction = Services.Feeds.IPriceFeed;

namespace Unit.TestHelpers
{
    /// <summary>
    /// Creates PortfolioService using whatever ctor the Services assembly currently exposes.
    /// Tries common patterns and falls back to reflection with sensible defaults.
    /// </summary>
    public static class ServiceFactory
    {
        public static PortfolioService CreatePortfolioService(string connString = "Data Source=:memory:")
        {
            // Common “real” dependencies we may need to pass
            var repo = new PortfoliosRepository(connString);
            var price = TryCreatePriceFeed();

            // Try typical matches first, in a predictable order
            var t = typeof(PortfolioService);
            var ctors = t.GetConstructors();

            // Prefer (PortfoliosRepository, IPriceFeed)
            var c = ctors.FirstOrDefault(x =>
            {
                var p = x.GetParameters();
                return p.Length == 2 &&
                       p[0].ParameterType == typeof(PortfoliosRepository) &&
                       typeof(FeedAbstraction).IsAssignableFrom(p[1].ParameterType);
            });
            if (c != null) return (PortfolioService)c.Invoke(new object[] { repo, price });

            // Try (string connectionString, IPriceFeed)
            c = ctors.FirstOrDefault(x =>
            {
                var p = x.GetParameters();
                return p.Length == 2 &&
                       p[0].ParameterType == typeof(string) &&
                       typeof(FeedAbstraction).IsAssignableFrom(p[1].ParameterType);
            });
            if (c != null) return (PortfolioService)c.Invoke(new object[] { connString, price });

            // Try (PortfoliosRepository) only
            c = ctors.FirstOrDefault(x =>
            {
                var p = x.GetParameters();
                return p.Length == 1 && p[0].ParameterType == typeof(PortfoliosRepository);
            });
            if (c != null) return (PortfolioService)c.Invoke(new object[] { repo });

            // Try (string connectionString) only
            c = ctors.FirstOrDefault(x =>
            {
                var p = x.GetParameters();
                return p.Length == 1 && p[0].ParameterType == typeof(string);
            });
            if (c != null) return (PortfolioService)c.Invoke(new object[] { connString });

            // Fallback: pick the shortest public ctor and build best-effort args
            c = ctors.OrderBy(x => x.GetParameters().Length).First();
            var args = c.GetParameters().Select(p =>
            {
                if (p.ParameterType == typeof(string)) return (object)connString;
                if (p.ParameterType == typeof(PortfoliosRepository)) return (object)repo;
                if (typeof(FeedAbstraction).IsAssignableFrom(p.ParameterType)) return (object)price;
                if (p.HasDefaultValue) return p.DefaultValue!;
                return p.ParameterType.IsValueType ? Activator.CreateInstance(p.ParameterType)! : null!;
            }).ToArray();

            return (PortfolioService)c.Invoke(args);
        }

        private static IPriceFeed TryCreatePriceFeed()
        {
            try { return new DummyPriceFeed(); }
            catch { throw new InvalidOperationException("Cannot create DummyPriceFeed; ensure Services.Feeds exists."); }
        }
    }
}
