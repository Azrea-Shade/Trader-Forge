using System;
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
        private readonly IEmailSender _email;
        private readonly IToastNotifier _toast;

        private Timer? _compileTimer;
        private Timer? _deliverTimer;

        public SchedulerService(SettingsService settings, BriefingService brief, IEmailSender email, IToastNotifier toast)
        {
            _settings = settings;
            _brief = brief;
            _email = email;
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

        private async Task CompileBriefAsync()
        {
            try
            {
                Log.Information("Compiling daily brief...");
                // For now, just log. Later, cache the compiled brief to disk.
                var sb = new StringBuilder();
                foreach (var line in _brief.GetMorningBriefStub())
                    sb.AppendLine($"• {line}");
                Log.Information("Brief compiled:\n{Brief}", sb.ToString());
                _toast.ShowInfo("Daily Brief", "Compiled. Will deliver at the scheduled time.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error compiling brief");
            }
        }

        private async Task DeliverBriefAsync()
        {
            try
            {
                Log.Information("Delivering daily brief...");
                var s = _settings.Load();
                var recipients = new[] { s.PrimaryEmail, s.SecondaryEmail };
                var html = "<h3>Daily Brief</h3><ul>" +
                           string.Join("", _brief.GetMorningBriefStub().Select(x => $"<li>{System.Net.WebUtility.HtmlEncode(x)}</li>")) +
                           "</ul>";
                await _email.SendAsync("Your Daily Brief", html, recipients);
                _toast.ShowInfo("Daily Brief", "Sent to your email recipients.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error delivering brief");
            }
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
