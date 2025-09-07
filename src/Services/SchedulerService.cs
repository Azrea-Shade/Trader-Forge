using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Serilog;

namespace Services
{
    public class SchedulerService : IDisposable
    {
        private readonly SettingsService _settings;
        private readonly BriefingService _brief;
        private readonly IToastNotifier _toast;

        private Timer? _compileTimer;
        private Timer? _deliverTimer;

        public SchedulerService(SettingsService settings, BriefingService brief, IToastNotifier toast)
        {
            _settings = settings;
            _brief = brief;
            _toast = toast;
        }

        public void Start()
        {
            var s = _settings.Load();
            ScheduleDaily(s);
        }

        public void ScheduleDaily(AppSettings s)
        {
            DisposeTimers();

            var tz = TimeZoneInfo.FindSystemTimeZoneById(s.TimeZoneId);
            var nowUtc = DateTimeOffset.UtcNow;

            var nextCompile = NextOccurrence(tz, s.CompileHour, s.CompileMinute, nowUtc);
            var nextDeliver = NextOccurrence(tz, s.DeliverHour, s.DeliverMinute, nowUtc);

            var dueCompile = nextCompile - nowUtc;
            var dueDeliver = nextDeliver - nowUtc;

            _compileTimer = new Timer(async _ => await CompileBriefAsync(), null, dueCompile, TimeSpan.FromDays(1));
            _deliverTimer = new Timer(async _ => await DeliverBriefAsync(), null, dueDeliver, TimeSpan.FromDays(1));

            Log.Information("Scheduler set. Compile at {CompileLocal}, Deliver at {DeliverLocal} ({Tz})",
                TimeZoneInfo.ConvertTime(nextCompile, tz).ToString("hh:mm tt"),
                TimeZoneInfo.ConvertTime(nextDeliver, tz).ToString("hh:mm tt"),
                s.TimeZoneId);
        }

        public static DateTimeOffset NextOccurrence(TimeZoneInfo tz, int hour, int minute, DateTimeOffset nowUtc)
        {
            var nowLocal = TimeZoneInfo.ConvertTime(nowUtc, tz);
            var targetLocal = new DateTimeOffset(nowLocal.Year, nowLocal.Month, nowLocal.Day, hour, minute, 0, tz.GetUtcOffset(nowLocal.DateTime));
            if (targetLocal <= nowLocal) targetLocal = targetLocal.AddDays(1);
            return TimeZoneInfo.ConvertTime(targetLocal, TimeZoneInfo.Utc);
        }

        private Task CompileBriefAsync()
        {
            try
            {
                Log.Information("Compiling daily brief...");
                var sb = new StringBuilder();
                foreach (var line in _brief.GetMorningBriefStub())
                    sb.AppendLine($"• {line}");

                // In future we could persist the compiled brief to disk.
                Log.Information("Brief compiled:\n{Brief}", sb.ToString());
                _toast.ShowInfo("Daily Brief", "Compiled. Will deliver at the scheduled time.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error compiling brief");
            }
            return Task.CompletedTask;
        }

        private Task DeliverBriefAsync()
        {
            try
            {
                Log.Information("Delivering daily brief (toast only)...");
                var lines = _brief.GetMorningBriefStub().ToArray();
                var first = lines.FirstOrDefault() ?? "Your brief is ready.";
                _toast.ShowInfo("Daily Brief", first);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error delivering brief");
            }
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            DisposeTimers();
        }

        private void DisposeTimers()
        {
            _compileTimer?.Dispose();
            _deliverTimer?.Dispose();
            _compileTimer = _deliverTimer = null;
        }
    }
}
