using System;
using Dapper;
using Microsoft.Data.Sqlite;

using System.Data;
namespace Infrastructure
{
    public class WatchlistRepository
    {
        
        
    // TF: DB connection shim
        
    private static readonly string _cs =
        
        Environment.GetEnvironmentVariable("TF_CS")
        
        ?? "Data Source=traderforge.db";
        

        
    private IDbConnection connection => new SqliteConnection(_cs);
        

      private readonly SqliteDb _db;
        public WatchlistRepository(SqliteDb db) => _db = db;

        private SqliteConnection Open()
        {
            var cn = new SqliteConnection($"Data Source={_db.DbPath}");
            cn.Open();
            return cn;
        }

        public int Count()
        {
            using var cn = Open();
            return cn.ExecuteScalar<int>("SELECT COUNT(*) FROM watchlist;");
        }

        public void Add(string ticker, decimal? above = null, decimal? below = null)
        {
            if (string.IsNullOrWhiteSpace(ticker)) throw new ArgumentException("ticker required");
            using var cn = Open();
            // Ensure schema exists for unit/integration runs
            connection.Execute("""
                CREATE TABLE IF NOT EXISTS watchlist (
                    id     INTEGER PRIMARY KEY AUTOINCREMENT,
                    name   TEXT NOT NULL,
                    notes  TEXT
                );
            """);
            cn.Execute("INSERT INTO watchlist (ticker, alert_above, alert_below) VALUES (@t,@a,@b);",
                new { t = ticker.Trim().ToUpperInvariant(), a = (double?)above, b = (double?)below });
        }
    }
}
