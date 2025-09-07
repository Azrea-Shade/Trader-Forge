using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;

namespace Infrastructure
{
    // NOTE: AlertRow record is defined elsewhere in Infrastructure. Do not duplicate here.
    // public record AlertRow(long Id, string Ticker, double? Above, double? Below, bool Enabled);

    public class AlertsRepository
    {
        private readonly SqliteDb _db;

        public AlertsRepository(SqliteDb db)
        {
            _db = db;
            Ensure();
        }

        private void Ensure()
        {
            const string sql = @"
CREATE TABLE IF NOT EXISTS alerts(
  Id      INTEGER PRIMARY KEY AUTOINCREMENT,
  Ticker  TEXT    NOT NULL,
  Above   REAL    NULL,
  Below   REAL    NULL,
  Enabled INTEGER NOT NULL DEFAULT 1
);";
            _db.Connection.Execute(sql);
        }

        public long Add(string ticker, double? above, double? below, bool enabled)
        {
            const string sql = @"INSERT INTO alerts(Ticker,Above,Below,Enabled) VALUES (@t,@a,@b,@e);
                                 SELECT last_insert_rowid();";
            var id = _db.Connection.ExecuteScalar<long>(sql, new
            {
                t = ticker,
                a = (object?)above,
                b = (object?)below,
                e = enabled ? 1 : 0
            });
            return id;
        }

        public void SetEnabled(long id, bool enabled)
        {
            const string sql = @"UPDATE alerts SET Enabled=@e WHERE Id=@id;";
            _db.Connection.Execute(sql, new { id, e = enabled ? 1 : 0 });
        }

        public IReadOnlyList<AlertRow> All()
        {
            const string sql = @"SELECT Id, Ticker, Above, Below, Enabled FROM alerts;";
            // Use dynamic + manual projection to avoid Dapper constructor mapping on records
            var rows = _db.Connection.Query(sql).ToList();
            var list = new List<AlertRow>(rows.Count);

            foreach (var r in rows)
            {
                long id      = Convert.ToInt64(r.Id);
                string tick  = (string)r.Ticker;

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
