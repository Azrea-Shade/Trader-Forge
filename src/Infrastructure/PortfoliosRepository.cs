using System;
using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;

namespace Infrastructure
{
    public partial class PortfoliosRepository
    {
        private readonly string _connectionString;

        public PortfoliosRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private T WithConnection<T>(Func<IDbConnection, T> work)
        {
            using var c = new SqliteConnection(_connectionString);
            c.Open();
            EnsureSchema(c);
            return work(c);
        }
        private void WithConnection(Action<IDbConnection> work)
        {
            using var c = new SqliteConnection(_connectionString);
            c.Open();
            EnsureSchema(c);
            work(c);
        }

        // Idempotent schema
        private static void EnsureSchema(IDbConnection conn)
        {
            conn.Execute(@"
CREATE TABLE IF NOT EXISTS portfolios (
    id    INTEGER PRIMARY KEY AUTOINCREMENT,
    name  TEXT NOT NULL,
    notes TEXT NULL
);

CREATE TABLE IF NOT EXISTS holdings (
    id           INTEGER PRIMARY KEY AUTOINCREMENT,
    portfolio_id INTEGER NOT NULL,
    symbol       TEXT NOT NULL,
    shares       REAL NOT NULL,
    cost         REAL NOT NULL DEFAULT 0,
    FOREIGN KEY (portfolio_id) REFERENCES portfolios(id) ON DELETE CASCADE
);
");
        }

        public int CreatePortfolio(string name, string? notes)
            => WithConnection(conn =>
                conn.ExecuteScalar<int>(
                    "INSERT INTO portfolios(name,notes) VALUES (@name,@notes); SELECT last_insert_rowid();",
                    new { name, notes }));

        public void UpdatePortfolio(long id, string name, string? notes)
            => WithConnection(conn =>
                conn.Execute("UPDATE portfolios SET name=@name, notes=@notes WHERE id=@id;",
                    new { id = checked((int)id), name, notes }));

        public void DeletePortfolio(long id)
            => WithConnection(conn => conn.Execute("DELETE FROM portfolios WHERE id=@id;", new { id = checked((int)id) }));
    }
}
