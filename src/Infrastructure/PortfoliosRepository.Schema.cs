using System.Data;
using Dapper;

namespace Infrastructure
{
    public partial class PortfoliosRepository
    {
        private static void EnsureSchema(IDbConnection connection)
        {
            connection.Execute(@"
CREATE TABLE IF NOT EXISTS portfolios (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  name TEXT NOT NULL UNIQUE,
  notes TEXT NULL
);

CREATE TABLE IF NOT EXISTS positions (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  portfolio_id INTEGER NOT NULL REFERENCES portfolios(id) ON DELETE CASCADE,
  ticker TEXT NOT NULL,
  qty REAL NOT NULL,
  avg_cost REAL NOT NULL,
  target_weight REAL NULL
);
");
        }
    }
}
