using System;
using Dapper;
using Microsoft.Data.Sqlite;

namespace Infrastructure
{
    public class WatchlistRepository
    {
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
            cn.Execute("INSERT INTO watchlist (ticker, alert_above, alert_below) VALUES (@t,@a,@b);",
                new { t = ticker.Trim().ToUpperInvariant(), a = (double?)above, b = (double?)below });
        }
    }
}
