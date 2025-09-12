using Services;
using Services.Feeds;
using System;using System.Linq;using Infrastructure;using Services;using Services.Feeds;

using System;
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
            var repo = new Infrastructure.PortfoliosRepository(connString);
            var price = TryCreatePriceFeed(); // Services.Feeds.Services::Feeds::Services.Feeds.IPriceFeed

            var t = typeof(PortfolioService);
            var ctors = t.GetConstructors();

            // Prefer (PortfoliosRepository, Services::Feeds::Services.Feeds.IPriceFeed)
            var c = ctors.FirstOrDefault(x =>
            {
                var p = x.GetParameters();
                return p.Length == 2 &&
                       p[0].ParameterType == typeof(PortfoliosRepository) &&
                       typeof(Services.Feeds.Services::Feeds::Services.Feeds.IPriceFeed).IsAssignableFrom(p[1].ParameterType);
            });
            if (c != null) return (PortfolioService)c.Invoke(new object[] { repo, price });

            // Try (string, Services::Feeds::Services.Feeds.IPriceFeed)
            c = ctors.FirstOrDefault(x =>
            {
                var p = x.GetParameters();
                return p.Length == 2 &&
                       p[0].ParameterType == typeof(string) &&
                       typeof(Services.Feeds.Services::Feeds::Services.Feeds.IPriceFeed).IsAssignableFrom(p[1].ParameterType);
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
                if (typeof(Services.Feeds.Services::Feeds::Services.Feeds.IPriceFeed).IsAssignableFrom(p.ParameterType)) return (object)price;
                if (p.HasDefaultValue) return p.DefaultValue!;
                return p.ParameterType.IsValueType ? Activator.CreateInstance(p.ParameterType)! : null!;
            }).ToArray();

            return (PortfolioService)c.Invoke(args);
        }

        private static Services.Feeds.Services::Feeds::Services.Feeds.IPriceFeed TryCreatePriceFeed()
        {
            try { return new Services.Feeds.DummyPriceFeed(); }
            catch { throw new InvalidOperationException("Cannot create Services.Feeds.DummyPriceFeed; ensure Services.Feeds exists."); }
        }
    }
}




namespace Unit.TestHelpers
{
    public static partial class ServiceFactory
    {
        // Ensure we always use THE SAME sqlite file the test passes in
        public static PortfolioService CreatePortfolioService(string dbfile)
        {
            var db   = new SqliteDb(dbfile);
            var repo = new Infrastructure.PortfoliosRepository(db);
            var feed = new Services.Feeds.DummyPriceFeed();
            return new Services.PortfolioService(repo, feed);
        }

        // Keep old call-sites working: single shared temp file for a run
        public static PortfolioService CreatePortfolioService()
        {
            var path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "tf_tests.sqlite");
            return CreatePortfolioService(path);
        }
    }
}
  