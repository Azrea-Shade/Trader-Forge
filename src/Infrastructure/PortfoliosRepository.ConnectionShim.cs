using System.Data;
using Dapper;

namespace Infrastructure;

public partial class PortfoliosRepository
{
    // Ensure the 'portfolios' table exists using the provided connection.
    private static void EnsureSchema(IDbConnection connection)
    {
        connection.Execute(@"
CREATE TABLE IF NOT EXISTS portfolios (
    id     INTEGER PRIMARY KEY AUTOINCREMENT,
    name   TEXT    NOT NULL,
    notes  TEXT    NULL
);");
    }
}
