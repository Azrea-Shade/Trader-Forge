using System.Text.Json;
using Microsoft.Data.Sqlite;
using Domain;

namespace Infrastructure
{
    public class SettingsRepository
    {
        private readonly SqliteDb _db;
        private const string Key = "app_settings";
        public SettingsRepository(SqliteDb db) => _db = db;

        public AppSettings LoadOrDefault()
        {
            using var cn = new SqliteConnection($"Data Source={_db.DbPath}");
            cn.Open();
            using var cmd = cn.CreateCommand();
            cmd.CommandText = "SELECT v FROM settings WHERE k=@k LIMIT 1;";
            cmd.Parameters.AddWithValue("@k", Key);
            var json = cmd.ExecuteScalar() as string;
            if (string.IsNullOrWhiteSpace(json)) return new AppSettings();
            try { return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings(); }
            catch { return new AppSettings(); }
        }

        public void Save(AppSettings s)
        {
            var json = JsonSerializer.Serialize(s);
            using var cn = new SqliteConnection($"Data Source={_db.DbPath}");
            cn.Open();
            using var up = cn.CreateCommand();
            up.CommandText = "INSERT INTO settings(k,v) VALUES(@k,@v) ON CONFLICT(k) DO UPDATE SET v=excluded.v;";
            up.Parameters.AddWithValue("@k", Key);
            up.Parameters.AddWithValue("@v", json);
            up.ExecuteNonQuery();
        }
    }
}
