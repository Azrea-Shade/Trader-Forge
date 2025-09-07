using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Microsoft.Data.Sqlite;

namespace Services
{
    public sealed record WatchlistDto(long Id, string Ticker, double? Above, double? Below);

    /// <summary>
    /// Minimal facade for Watchlist operations, using SqliteDb.DbPath directly.
    /// Keeps existing repositories/services untouched.
    /// </summary>
    public class WatchlistFacade
    {
        private readonly Infrastructure.SqliteDb _db;
        public WatchlistFacade(Infrastructure.SqliteDb db) => _db = db;

        private IDbConnection Open() => new SqliteConnection($"Data Source={_db.DbPath}");

        public long Add(string ticker)
        {
            const string sql = @"INSERT INTO watchlist(ticker, alert_above, alert_below) VALUES(@t, NULL, NULL);
                                 SELECT last_insert_rowid();";
            using var con = Open();
            return con.ExecuteScalar<long>(sql, new { t = ticker });
        }

        public void Remove(long id)
        {
            const string sql = @"DELETE FROM watchlist WHERE id=@id;";
            using var con = Open();
            con.Execute(sql, new { id });
        }

        public void SetThresholds(long id, double? above, double? below)
        {
            const string sql = @"UPDATE watchlist SET alert_above=@a, alert_below=@b WHERE id=@id;";
            using var con = Open();
            con.Execute(sql, new { id, a = (object?)above, b = (object?)below });
        }

        public IReadOnlyList<WatchlistDto> All()
        {
            const string sql = @"SELECT id AS Id, ticker AS Ticker, alert_above AS Above, alert_below AS Below
                                 FROM watchlist ORDER BY ticker ASC;";
            using var con = Open();
            return con.Query<WatchlistDto>(sql).ToList();
        }
    }
}
