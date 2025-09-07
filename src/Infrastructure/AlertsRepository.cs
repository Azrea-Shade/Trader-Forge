using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Microsoft.Data.Sqlite;

namespace Infrastructure
{
    // Use the canonical AlertRow model from AlertRow.cs

    public class AlertsRepository
    {
        private readonly SqliteDb _db;

        public AlertsRepository(SqliteDb db)
        {
            _db = db; // SqliteDb ensures schema & defaults in its ctor
        }

        private IDbConnection Open() => new SqliteConnection($"Data Source={_db.DbPath}");

        public long Add(string ticker, double? above, double? below, bool enabled)
        {
            const string sql = @"
INSERT INTO alerts(ticker, above, below, enabled)
VALUES (@t, @a, @b, @e);
SELECT last_insert_rowid();";
            using var con = Open();
            return con.ExecuteScalar<long>(sql, new
            {
                t = ticker,
                a = (object?)above,
                b = (object?)below,
                e = enabled ? 1 : 0
            });
        }

        public void SetEnabled(long id, bool enabled)
        {
            const string sql = @"UPDATE alerts SET enabled = @e WHERE id = @id;";
            using var con = Open();
            con.Execute(sql, new { id, e = enabled ? 1 : 0 });
        }

        public int Count()
        {
            const string sql = @"SELECT COUNT(*) FROM alerts;";
            using var con = Open();
            return con.ExecuteScalar<int>(sql);
        }

        public IReadOnlyList<AlertRow> All()
        {
            const string sql = @"
SELECT
  id      AS Id,
  ticker  AS Ticker,
  above   AS Above,
  below   AS Below,
  enabled AS Enabled
FROM alerts;";
            using var con = Open();

            // Manual projection to avoid ctor-mapping quirks on records
            var rows = con.Query(sql).ToList();
            var list = new List<AlertRow>(rows.Count);

            foreach (var r in rows)
            {
                long id = Convert.ToInt64(r.Id);
                string tick = (string)r.Ticker;

                double? above = null;
                if (!(r.Above is null) && !(r.Above is DBNull))
                    above = Convert.ToDouble(r.Above);

                double? below = null;
                if (!(r.Below is null) && !(r.Below is DBNull))
                    below = Convert.ToDouble(r.Below);

                bool enabled = r.Enabled is bool b ? b : Convert.ToInt64(r.Enabled) != 0;

                list.Add(new AlertRow(id, tick, above, below, enabled));
            }

            return list;
        }
    }
}
