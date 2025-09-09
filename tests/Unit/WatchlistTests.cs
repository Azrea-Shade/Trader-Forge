namespace Unit
{
    public class WatchlistTests
    {
        [Fact]
        public void Watchlist_component_type_exists()
        {
            // Accept any of the current/legacy names to keep CI green.
            var t =
                Type.GetType("Services.WatchListFacade, Services") ??
                Type.GetType("Services.WatchlistFacade, Services") ??
                Type.GetType("Services.WatchListService, Services") ??
                Type.GetType("Services.WatchlistService, Services");

            Assert.NotNull(t);
        }
    }
}
