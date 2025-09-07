using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace Infrastructure
{
    public class SqliteDb
    {
        public string DbPath { get; }
        public SqliteDb(string? dbPathOverride = null)
        {
            var baseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AzreaCompanion");
            Directory.CreateDirectory(baseDir);
            DbPath = dbPathOverride ?? Path.Combine(baseDir, "app.db");
            EnsureSchema();
        }

        private void EnsureSchema()
        {
            using var cn = new SqliteConnection($"Data Source={DbPath}");
            cn.Open();
            using var cmd = cn.CreateCommand();
            cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS settings (k TEXT PRIMARY KEY, v TEXT);
CREATE TABLE IF NOT EXISTS watchlist (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  ticker TEXT NOT NULL,
  alert_above REAL NULL,
  alert_below REAL NULL
);";
            cmd.ExecuteNonQuery();
        }
    }
}
