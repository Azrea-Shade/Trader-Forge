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
);

-- Phase 5: portfolios & holdings
CREATE TABLE IF NOT EXISTS portfolios (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  name TEXT NOT NULL UNIQUE,
  notes TEXT NULL
);

CREATE TABLE IF NOT EXISTS holdings (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  portfolio_id INTEGER NOT NULL,
  ticker TEXT NOT NULL,
  shares REAL NOT NULL DEFAULT 0,
  cost_basis REAL NOT NULL DEFAULT 0, -- total cost for these shares
  FOREIGN KEY (portfolio_id) REFERENCES portfolios(id) ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS ix_holdings_portfolio ON holdings(portfolio_id);
CREATE INDEX IF NOT EXISTS ix_holdings_ticker ON holdings(ticker);
";
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

            // Optional seed: default portfolio
            using (var c2 = cn.CreateCommand())
            {
                c2.Transaction = tx;
                c2.CommandText = @"INSERT INTO portfolios(name,notes)
                                   SELECT 'My Portfolio','Default portfolio'
                                   WHERE NOT EXISTS(SELECT 1 FROM portfolios WHERE name='My Portfolio');";
                c2.ExecuteNonQuery();
            }

            tx.Commit();
        }
    }
}
