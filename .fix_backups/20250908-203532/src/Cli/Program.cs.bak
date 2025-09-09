using System;
using System.Threading.Tasks;
using Infrastructure;
using Services;
using Services;

// Simple CLI: default "OK" for smoke; `brief generate|notify` for daily brief.
if (args.Length >= 1 && args[0].Equals("brief", StringComparison.OrdinalIgnoreCase))
{
    var db = new SqliteDb();
    var feed = new DummyPriceFeed(); // CI-safe, no network
    var svc = new BriefingService(db, feed);
    var today = DateOnly.FromDateTime(DateTime.Today);

    if (args.Length >= 2 && args[1].Equals("generate", StringComparison.OrdinalIgnoreCase))
    {
        var content = await svc.GenerateAsync(today);
        Console.WriteLine("✅ Generated daily brief.\n");
        Console.WriteLine(content);
        return;
    }
    if (args.Length >= 2 && args[1].Equals("notify", StringComparison.OrdinalIgnoreCase))
    {
        svc.MarkDelivered(today);
        Console.WriteLine("🔔 Notify stub (CI-safe): Today's brief marked delivered.");
        return;
    }

    Console.WriteLine("Usage: cli brief generate|notify");
    return;
}

// Default smoke path
Console.WriteLine("OK");
