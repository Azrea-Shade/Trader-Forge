using System;
using System.IO;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Domain;
using Integrations;
using Services;

namespace AzreaCompanion
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = null!;
        private IHost? _host;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var logs = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AzreaCompanion", "logs");
            Directory.CreateDirectory(logs);
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(Path.Combine(logs, "app-.log"), rollingInterval: RollingInterval.Day)
                .CreateLogger();

            _host = Host.CreateDefaultBuilder()
                .UseSerilog()
                .ConfigureServices((ctx, services) =>
                {
                    // Infrastructure & storage
                    services.AddSingleton<Infrastructure.SqliteDb>();
                    services.AddSingleton<Infrastructure.WatchlistRepository>();
                    services.AddSingleton<Infrastructure.SettingsRepository>();

                    // Core services
                    services.AddSingleton<Services.WatchlistService>();
                    services.AddSingleton<Services.BriefingService>();
                    services.AddSingleton<Services.SettingsService>();

                    // Notifications (toast only)
                    services.AddSingleton<Services.IToastNotifier, AzreaCompanion.WinToastNotifier>();

                    // Scheduler
                    services.AddSingleton<Services.SchedulerService>();

                    // HTTP (for future providers)
                    services.AddHttpClient();

                    // ViewModels & Window
                    services.AddSingleton<Presentation.MainViewModel>();
                    services.AddSingleton<MainWindow>();

                    // Autostart helper
                    services.AddSingleton<AutostartService>();

                    // Quote provider (dummy for now)
                    services.AddSingleton<Domain.IQuoteProvider, Integrations.DummyQuoteProvider>();
                })
                .Build();

            _host.Start();
            Services = _host.Services;

            // Autostart if enabled
            var settings = Services.GetRequiredService<Services.SettingsService>().Load();
            if (settings.AutostartOnLogin)
            {
                Services.GetRequiredService<AutostartService>().EnsureAutostart();
            }

            // Start daily scheduler
            Services.GetRequiredService<Services.SchedulerService>().Start();

            var window = Services.GetRequiredService<MainWindow>();
            window.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            (_host as IDisposable)?.Dispose();
            Log.CloseAndFlush();
            base.OnExit(e);
        }
    }
}
