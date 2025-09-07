using Dapper;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace Infrastructure
{
    public class FavoritesRepository
    {
        private readonly SqliteDb _db;
        public FavoritesRepository(SqliteDb db) => _db = db;

        private SqliteConnection Open() { var cn = new SqliteConnection($"Data Source={_db.DbPath}"); cn.Open(); return cn; }

        public int Count()
        {
            using var cn = Open();
            return cn.ExecuteScalar<int>("SELECT COUNT(*) FROM favorites;");
        }

        public void Add(string ticker)
        {
            using var cn = Open();
            // Ensure schema exists for unit/integration runs
            connection.Execute("""
                CREATE TABLE IF NOT EXISTS favorites (
                    id     INTEGER PRIMARY KEY AUTOINCREMENT,
                    name   TEXT NOT NULL,
                    notes  TEXT
                );
            """);
            cn.Execute("INSERT OR IGNORE INTO favorites(ticker) VALUES(@t);", new { t = ticker.Trim().ToUpperInvariant() });
        }

        public void Remove(string ticker)
        {
            using var cn = Open();
            cn.Execute("DELETE FROM favorites WHERE ticker=@t;", new { t = ticker.Trim().ToUpperInvariant() });
        }

        public IEnumerable<string> All()
        {
            using var cn = Open();
            return cn.Query<string>("SELECT ticker FROM favorites ORDER BY ticker;");
        }
    }
}
