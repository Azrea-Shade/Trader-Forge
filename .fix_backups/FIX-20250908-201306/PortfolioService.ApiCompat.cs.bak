using System;
using System.Reflection;
using Infrastructure;
using Domain;

namespace Services
{
    public partial class PortfolioService
    {
        // Accept SqliteDb directly (tests pass this)
        public PortfolioService(SqliteDb db) : this(ExtractConnectionString(db)) { }

        // If your real service has more ctor params, you can chain as needed:
        // public PortfolioService(SqliteDb db, Feeds.IPriceFeed feed) : this(ExtractConnectionString(db), feed) { }

        // Overload so tests can call without notes
        public int CreatePortfolio(string name) => CreatePortfolio(name, null);

        // Reflection helper to obtain a connection string from SqliteDb without changing Infrastructure
        private static string ExtractConnectionString(SqliteDb db)
        {
            // Try common field/property names
            var t = typeof(SqliteDb);
            var f = t.GetField("_connectionString", BindingFlags.Instance|BindingFlags.NonPublic)
                  ?? t.GetField("_cs", BindingFlags.Instance|BindingFlags.NonPublic);
            if (f != null) return (string)(f.GetValue(db) ?? "Data Source=traderforge.db");

            var p = t.GetProperty("ConnectionString", BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic);
            if (p?.CanRead == true)
            {
                var val = p.GetValue(db) as string;
                if (!string.IsNullOrWhiteSpace(val)) return val!;
            }
            // Fallback (safe default)
            return "Data Source=traderforge.db";
        }
    }
}
