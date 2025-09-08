using System.Data;
using Microsoft.Data.Sqlite;

namespace Infrastructure
{
    public partial class PortfoliosRepository
    {
        // Backing field used by both the property and any legacy direct references.
        private SqliteConnection? _connection;

        // Provide the IDbConnection expected by existing repository methods.
        private IDbConnection connection
        {
            get
            {
                if (_connection == null)
                {
                    // Assumes _connectionString already exists in the main class (as in prior phases).
                    var c = new SqliteConnection(_connectionString);
                    c.Open();
                    _connection = c;
                }
                return _connection;
            }
        }
    }
}
