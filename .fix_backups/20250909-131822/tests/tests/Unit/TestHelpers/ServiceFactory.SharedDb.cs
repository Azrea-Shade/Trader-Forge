using Infrastructure;
using Services;
using Services.Feeds;

namespace Unit.TestHelpers
{
    public static partial class ServiceFactory
    {
        public static PortfolioService CreatePortfolioService(string dbfile)
        {
            var db   = new SqliteDb(dbfile);
            var repo = new Infrastructure.PortfoliosRepository(db);
            var feed = new Services.Feeds.DummyPriceFeed();
            return new Services.PortfolioService(repo, feed);
        }

        public static PortfolioService CreatePortfolioService()
        {
            var path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "tf_tests.sqlite");
            return CreatePortfolioService(path);
        }
    }
}
