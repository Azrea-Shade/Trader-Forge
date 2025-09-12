using Services;
namespace Services
{
    public class AppPathsService
    {
        private readonly Infrastructure.SqliteDb _db;
        public AppPathsService(Infrastructure.SqliteDb db) => _db = db;

        public string DbPath => _db.DbPath;
    }
}
