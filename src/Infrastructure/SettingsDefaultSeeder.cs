using Microsoft.Data.Sqlite;

namespace Infrastructure
{
    /// <summary>
    /// Ensures certain defaults exist even if a user's DB predates new settings.
    /// Called lazily by components that touch settings.
    /// </summary>
    public static class SettingsDefaultSeeder
    {
        public static void EnsureDefaults(SqliteDb db)
        {
            using var cn = new SqliteConnection($"Data Source={db.DbPath}");
            cn.Open();
            using var cmd = cn.CreateCommand();
            cmd.CommandText = @"INSERT INTO settings(k,v) VALUES('Notifications.Toasts','On')
                                ON CONFLICT(k) DO NOTHING;";
            cmd.ExecuteNonQuery();
        }
    }
}
