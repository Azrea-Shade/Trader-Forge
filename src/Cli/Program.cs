using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Services.Engines;
using Services.Feeds;

namespace Cli
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            try
            {
                var tickers = args.Length > 0 ? args : new[] { "AAPL", "MSFT", "SPY" };
                var feed = new DummyPriceFeed();
                var prices = await feed.GetPricesAsync(tickers);

                var briefLines = BriefingEngine.BuildBrief(tickers, prices, DateTime.UtcNow);

                var outDir = Path.Combine(AppContext.BaseDirectory, "cli_out");
                Directory.CreateDirectory(outDir);
                var jsonPath = Path.Combine(outDir, "brief.json");

                var payload = new
                {
                    generatedUtc = DateTime.UtcNow,
                    tickers,
                    prices,
                    lines = briefLines
                };

                var opts = new JsonSerializerOptions { WriteIndented = true };
                File.WriteAllText(jsonPath, JsonSerializer.Serialize(payload, opts));

                Console.WriteLine($"CLI brief written: {jsonPath}");
                foreach (var line in briefLines)
                    Console.WriteLine(line);

                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return 1;
            }
        }
    }
}
