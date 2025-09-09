using System;
using System.Linq;
using Infrastructure;
using Services;
using Services.Feeds;

namespace Unit.TestHelpers
{
    /// <summary>
    /// Creates PortfolioService using whatever ctor the assembly currently exposes.
    /// Tries common signatures, falls back to shortest public ctor with best-guess args.
    /// </summary>
    public static class ServiceFactory
    {
        public static PortfolioService CreatePortfolioService(string connString = "Data Source=:memory:")
        {
            var repo = new PortfoliosRepository(connString);
            var price = TryCreatePriceFeed(); // Services.Feeds.IPriceFeed

            var t = typeof(PortfolioService);
            var ctors = t.GetConstructors();

            // Prefer (PortfoliosRepository, IPriceFeed)
            var c = ctors.FirstOrDefault(x =>
            {
                var p = x.GetParameters();
                return p.Length == 2 &&
                       p[0].ParameterType == typeof(PortfoliosRepository) &&
                       typeof(Services.Feeds.IPriceFeed).IsAssignableFrom(p[1].ParameterType);
            });
            if (c != null) return (PortfolioService)c.Invoke(new object[] { repo, price });

            // Try (string, IPriceFeed)
            c = ctors.FirstOrDefault(x =>
            {
                var p = x.GetParameters();
                return p.Length == 2 &&
                       p[0].ParameterType == typeof(string) &&
                       typeof(Services.Feeds.IPriceFeed).IsAssignableFrom(p[1].ParameterType);
            });
            if (c != null) return (PortfolioService)c.Invoke(new object[] { connString, price });

            // Try (PortfoliosRepository)
            c = ctors.FirstOrDefault(x =>
            {
                var p = x.GetParameters();
                return p.Length == 1 && p[0].ParameterType == typeof(PortfoliosRepository);
            });
            if (c != null) return (PortfolioService)c.Invoke(new object[] { repo });

            // Try (string)
            c = ctors.FirstOrDefault(x =>
            {
                var p = x.GetParameters();
                return p.Length == 1 && p[0].ParameterType == typeof(string);
            });
            if (c != null) return (PortfolioService)c.Invoke(new object[] { connString });

            // Fallback: shortest ctor, best-effort arg mapping
            c = ctors.OrderBy(x => x.GetParameters().Length).First();
            var args = c.GetParameters().Select(p =>
            {
                if (p.ParameterType == typeof(string)) return (object)connString;
                if (p.ParameterType == typeof(PortfoliosRepository)) return (object)repo;
                if (typeof(Services.Feeds.IPriceFeed).IsAssignableFrom(p.ParameterType)) return (object)price;
                if (p.HasDefaultValue) return p.DefaultValue!;
                return p.ParameterType.IsValueType ? Activator.CreateInstance(p.ParameterType)! : null!;
            }).ToArray();

            return (PortfolioService)c.Invoke(args);
        }

        private static Services.Feeds.IPriceFeed TryCreatePriceFeed()
        {
            try { return new DummyPriceFeed(); }
            catch { throw new InvalidOperationException("Cannot create DummyPriceFeed; ensure Services.Feeds exists."); }
        }
    }
}
