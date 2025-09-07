using System.Data;
using Dapper;

namespace Infrastructure
{
    public partial class PortfoliosRepository
    {
        private void EnsureSchema()
        {
            const string sql =
@"CREATE TABLE IF NOT EXISTS portfolios(
    id      INTEGER PRIMARY KEY AUTOINCREMENT,
    name    TEXT NOT NULL,
    notes   TEXT NULL
);";
            _connection.Execute(sql);
        }
    }
}
