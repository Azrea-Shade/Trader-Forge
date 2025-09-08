using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;

namespace Infrastructure
{
    public class PortfoliosRepository
    {
        private readonly string _connectionString;

        public PortfoliosRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Centralized connection helper (always ensures schema)
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

        // Schema guard kept here (no external shim needed)
        private static void EnsureSchema(IDbConnection conn)
        {
            conn.Execute(@"
CREATE TABLE IF NOT EXISTS portfolios (
    id     INTEGER PRIMARY KEY AUTOINCREMENT,
    name   TEXT NOT NULL,
    notes  TEXT NULL
);");
        }

        public int CreatePortfolio(string name, string notes)
            => WithConnection(conn =>
                conn.ExecuteScalar<int>(
                    "INSERT INTO portfolios(name,notes) VALUES (@name,@notes); SELECT last_insert_rowid();",
                    new { name, notes }));

        public void UpdatePortfolio(int id, string name, string notes)
            => WithConnection(conn =>
                conn.Execute("UPDATE portfolios SET name=@name, notes=@notes WHERE id=@id;",
                    new { id, name, notes }));

        public void DeletePortfolio(int id)
            => WithConnection(conn =>
                conn.Execute("DELETE FROM portfolios WHERE id=@id;", new { id }));

        // If callers need to list portfolios; harmless if unused.
        public IEnumerable<(int id, string name, string notes)> ListPortfolios()
            => WithConnection(conn =>
                conn.Query<(int id, string name, string notes)>(
                    "SELECT id, name, COALESCE(notes,'') AS notes FROM portfolios ORDER BY id;"));
    }
}
