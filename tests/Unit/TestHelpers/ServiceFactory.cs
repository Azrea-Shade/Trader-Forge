using Xunit;
using System;
using System.IO;
using Infrastructure;
using Services;
using Services.Feeds;

namespace Unit.TestHelpers
{
    public static partial class ServiceFactory
    {
        // Primary: allow tests to pass an explicit db file path
        public static PortfolioService CreatePortfolioService(string dbfile)
        {
            var dir = Path.GetDirectoryName(dbfile);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

            // Use plain connection string (PortfoliosRepository takes string)
            var cs = $"Data Source={dbfile}";
            var repo = new Infrastructure.PortfoliosRepository(cs);

            Services.Feeds.IPriceFeed feed = new Services.Feeds.DummyPriceFeed();
            return new Services.PortfolioService(repo, feed);
        }

        // Convenience: shared temp db for a test run
        public static PortfolioService CreatePortfolioService()
        {
            var path = Path.Combine(Path.GetTempPath(), "tf_tests.sqlite");
            return CreatePortfolioService(path);
        }

        // Helper for alert-engine tests
        public static Services.Engines.AlertEngine CreateAlertEngine()
        {
            Services.Feeds.IPriceFeed feed = new Services.Feeds.DummyPriceFeed();
            return new Services.Engines.AlertEngine(feed);
        }
    }
}
