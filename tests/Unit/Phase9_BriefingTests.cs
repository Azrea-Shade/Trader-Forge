using System;
using System.IO;
using System.Threading.Tasks;
using Infrastructure;
using Services;
using Services;
using Xunit;

public class Phase9_BriefingTests
{
    [Fact]
    public async Task Brief_engine_writes_and_reads_latest()
    {
        var path = Path.Combine(Path.GetTempPath(), $"tf_brief_{Guid.NewGuid():N}.db");
        try
        {
            var db = new SqliteDb(path);
            var svc = new BriefingService(db, new DummyPriceFeed());
            var today = DateOnly.FromDateTime(DateTime.Today);

            var content = await svc.GenerateAsync(today);
            Assert.Contains("Daily Brief", content);

            var repo = new BriefsRepository(db);
            var latest = repo.Latest();
            Assert.True(latest.HasValue);
            Assert.Contains(today.ToString("yyyy-MM-dd"), latest.Value.date);
            Assert.False(string.IsNullOrWhiteSpace(latest.Value.content));
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }
}
