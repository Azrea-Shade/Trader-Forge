using Dapper;
using Microsoft.Data.Sqlite;

namespace Infrastructure
{
    public class SettingsRepository
    {
        private readonly SqliteDb _db;
        public SettingsRepository(SqliteDb db) => _db = db;

        private SqliteConnection Open() { var cn = new SqliteConnection($"Data Source={_db.DbPath}"); cn.Open(); return cn; }

        public string? Get(string key)
        {
            using var cn = Open();
            return cn.ExecuteScalar<string?>("SELECT v FROM settings WHERE k=@k;", new { k = key });
        }

        public void Set(string key, string value)
        {
            using var cn = Open();
            cn.Execute(@"INSERT INTO settings(k,v) VALUES(@k,@v)
                         ON CONFLICT(k) DO UPDATE SET v=excluded.v;", new { k = key, v = value });
        }
    }
}
