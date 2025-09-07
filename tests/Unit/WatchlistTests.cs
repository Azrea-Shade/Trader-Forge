using System;
using System.IO;
using FluentAssertions;
using Xunit;

public class WatchlistTests
{
    [Fact]
    public void Add_and_Count_Works()
    {
        var tmp = Path.Combine(Path.GetTempPath(), "azrea_tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tmp);
        var dbFile = Path.Combine(tmp, "test.db");

        var db = new Infrastructure.SqliteDb(dbFile);
        var repo = new Infrastructure.WatchlistRepository(db);
        var svc  = new Services.WatchlistService(repo);

        svc.GetCount().Should().Be(0);
        svc.AddSampleMsft().Should().Be(1);
        svc.GetCount().Should().Be(1);
    }
}
