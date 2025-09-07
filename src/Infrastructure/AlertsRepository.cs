using Dapper;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace Infrastructure
{
    public record AlertRow(long Id, string Ticker, double? Above, double? Below, bool Enabled);

    public class AlertsRepository
    {
        private readonly SqliteDb _db;
        public AlertsRepository(SqliteDb db) => _db = db;

        private SqliteConnection Open() { var cn = new SqliteConnection($"Data Source={_db.DbPath}"); cn.Open(); return cn; }

        public int Count()
        {
            using var cn = Open();
            return cn.ExecuteScalar<int>("SELECT COUNT(*) FROM alerts;");
        }

        public long Add(string ticker, double? above = null, double? below = null, bool enabled = true)
        {
            using var cn = Open();
            return cn.ExecuteScalar<long>(
                @"INSERT INTO alerts(ticker, above, below, enabled) VALUES(@t,@a,@b,@e);
                  SELECT last_insert_rowid();",
                new { t = ticker.Trim().ToUpperInvariant(), a = above, b = below, e = enabled ? 1 : 0 });
        }

        public IEnumerable<AlertRow> All()
        {
            using var cn = Open();
            return cn.Query<AlertRow>("SELECT id as Id, ticker as Ticker, above as Above, below as Below, enabled as Enabled FROM alerts;");
        }

        public void SetEnabled(long id, bool enabled)
        {
            using var cn = Open();
            cn.Execute("UPDATE alerts SET enabled=@e WHERE id=@id;", new { e = enabled ? 1 : 0, id });
        }
    }
}
