using System;
using System.IO;
using System.Text.Json;

var outDir = Path.Combine(Directory.GetCurrentDirectory(), "cli_out");
Directory.CreateDirectory(outDir);

var dbPath = Path.Combine(outDir, "cli.db");
var db = new Infrastructure.SqliteDb(dbPath);

var repo = new Infrastructure.WatchlistRepository(db);
var svc  = new Services.WatchlistService(repo);

var before = svc.GetCount();
var after  = svc.AddSampleMsft();

var result = new {
    timestamp = DateTimeOffset.UtcNow,
    db = db.DbPath,
    before, after,
    added = "MSFT"
};

var json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
File.WriteAllText(Path.Combine(outDir, "result.json"), json);
Console.WriteLine(json);
