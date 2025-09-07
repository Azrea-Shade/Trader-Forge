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
            SeedDefaults();
        }

        private void EnsureSchema()
        {
            using var cn = new SqliteConnection($"Data Source={DbPath}");
            cn.Open();
            using var cmd = cn.CreateCommand();
            cmd.CommandText = @"
PRAGMA journal_mode=WAL;

CREATE TABLE IF NOT EXISTS settings (
  k TEXT PRIMARY KEY,
  v TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS watchlist (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  ticker TEXT NOT NULL,
  alert_above REAL NULL,
  alert_below REAL NULL
);

CREATE TABLE IF NOT EXISTS favorites (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  ticker TEXT NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS alerts (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  ticker TEXT NOT NULL,
  above REAL NULL,
  below REAL NULL,
  enabled INTEGER NOT NULL DEFAULT 1
);";
            cmd.ExecuteNonQuery();
        }

        private void SeedDefaults()
        {
            using var cn = new SqliteConnection($"Data Source={DbPath}");
            cn.Open();
            using var tx = cn.BeginTransaction();

            void upsert(string k, string v)
            {
                using var c = cn.CreateCommand();
                c.Transaction = tx;
                c.CommandText = @"INSERT INTO settings(k,v) VALUES(@k,@v)
                                  ON CONFLICT(k) DO NOTHING;";
                c.Parameters.AddWithValue("@k", k);
                c.Parameters.AddWithValue("@v", v);
                c.ExecuteNonQuery();
            }

            upsert("Brief.GenerateAt", "07:30");
            upsert("Brief.NotifyAt", "08:00");
            upsert("Autostart", "On");

            tx.Commit();
        }
    }
}
