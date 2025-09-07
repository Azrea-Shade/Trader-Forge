using System;
using Microsoft.Data.Sqlite;

namespace Infrastructure
{
    public class BriefsRepository
    {
        private readonly SqliteDb _db;
        public BriefsRepository(SqliteDb db) => _db = db;

        public void UpsertGenerated(string date, string content, DateTimeOffset generatedAt)
        {
            using var cn = new SqliteConnection($"Data Source={_db.DbPath}");
            cn.Open();
            using var cmd = cn.CreateCommand();
            cmd.CommandText = @"INSERT INTO briefs(date, generated_at, content)
                                VALUES(@d,@g,@c)
                                ON CONFLICT(date) DO UPDATE SET generated_at=@g, content=@c;";
            cmd.Parameters.AddWithValue("@d", date);
            cmd.Parameters.AddWithValue("@g", generatedAt.ToString("o"));
            cmd.Parameters.AddWithValue("@c", content);
            cmd.ExecuteNonQuery();
        }

        public void MarkDelivered(string date, DateTimeOffset deliveredAt)
        {
            using var cn = new SqliteConnection($"Data Source={_db.DbPath}");
            cn.Open();
            using var cmd = cn.CreateCommand();
            cmd.CommandText = @"UPDATE briefs SET delivered_at=@da WHERE date=@d;";
            cmd.Parameters.AddWithValue("@d", date);
            cmd.Parameters.AddWithValue("@da", deliveredAt.ToString("o"));
            cmd.ExecuteNonQuery();
        }

        public (string date, string content)? Latest()
        {
            using var cn = new SqliteConnection($"Data Source={_db.DbPath}");
            cn.Open();
            using var cmd = cn.CreateCommand();
            cmd.CommandText = @"SELECT date, content FROM briefs ORDER BY date DESC LIMIT 1;";
            using var rdr = cmd.ExecuteReader();
            if (!rdr.Read()) return null;
            return (rdr.GetString(0), rdr.GetString(1));
        }
    }
}
