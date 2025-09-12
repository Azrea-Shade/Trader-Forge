using System.Collections.Generic;
using Dapper;
using Microsoft.Data.Sqlite;

namespace Infrastructure
{
    public class WatchlistReader
    {
        private readonly SqliteDb _db;
        public WatchlistReader(SqliteDb db) => _db = db;

        private SqliteConnection Open()
        {
            var cn = new SqliteConnection($"Data Source={_db.DbPath}");
            cn.Open();
            return cn;
        }

        public IReadOnlyList<string> AllTickers()
        {
            using var cn = Open();
            return cn.Query<string>("SELECT ticker FROM watchlist ORDER BY ticker;").AsList();
        }
    }
}
