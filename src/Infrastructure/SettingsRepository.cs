using Dapper;
using Microsoft.Data.Sqlite;

using System.Data;
namespace Infrastructure
{
    public class SettingsRepository
    {
        
        
    // TF: DB connection shim
        
    private static readonly string _cs =
        
        Environment.GetEnvironmentVariable("TF_CS")
        
        ?? "Data Source=traderforge.db";
        

        
    private IDbConnection connection => new SqliteConnection(_cs);
        

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
            // Ensure schema exists for unit/integration runs
            connection.Execute("""
                CREATE TABLE IF NOT EXISTS settings (
                    id     INTEGER PRIMARY KEY AUTOINCREMENT,
                    name   TEXT NOT NULL,
                    notes  TEXT
                );
            """);
            cn.Execute(@"INSERT INTO settings(k,v) VALUES(@k,@v)
                         ON CONFLICT(k) DO UPDATE SET v=excluded.v;", new { k = key, v = value });
        }
    }
}
