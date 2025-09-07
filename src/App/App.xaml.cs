using System;
using System.IO;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Domain;
using Integrations;

namespace AzreaCompanion
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = null!;
        private IHost? _host;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var logs = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "AzreaCompanion",
                "logs");
            Directory.CreateDirectory(logs);

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(Path.Combine(logs, "app-.log"), rollingInterval: RollingInterval.Day)
                .CreateLogger();

            _host = Host.CreateDefaultBuilder()
                .UseSerilog()
                .ConfigureServices((ctx, services) =>
                {
                    // Infrastructure
                    services.AddSingleton<Infrastructure.SqliteDb>();
                    services.AddSingleton<Infrastructure.WatchlistRepository>();
                    services.AddSingleton<Infrastructure.FavoritesRepository>();
                    services.AddSingleton<Infrastructure.SettingsRepository>();
                    services.AddSingleton<Infrastructure.AlertsRepository>();

                    // Services (Phase 1)
                    services.AddSingleton<Services.WatchlistService>();
                    services.AddSingleton<Services.FavoritesService>();
                    services.AddSingleton<Services.SettingsService>();
                    services.AddSingleton<Services.AlertsService>();
                    services.AddSingleton<Services.AppPathsService>();

                    // Engines / HTTP (Phase 2 ready)
                    services.AddHttpClient();
                    services.AddSingleton<Services.BriefingService>();
                    services.AddSingleton<IQuoteProvider, DummyQuoteProvider>();

                    // Page VMs (Phase 3)
                    services.AddSingleton<Presentation.DashboardViewModel>();
                    services.AddSingleton<Presentation.WatchlistViewModel>();
                    services.AddSingleton<Presentation.FavoritesViewModel>();
                    services.AddSingleton<Presentation.AlertsViewModel>();
                    services.AddSingleton<Presentation.SettingsViewModel>();

                    // Shell + About
                    services.AddSingleton<Presentation.MainViewModel>();
                    services.AddSingleton<Presentation.AboutViewModel>();

                    // Windows
                    services.AddSingleton<MainWindow>();
                    services.AddTransient<AboutWindow>();
                })
                .Build();

            _host.Start();
            Services = _host.Services;

            var window = Services.GetRequiredService<MainWindow>();
            window.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _host?.Dispose();
            Log.CloseAndFlush();
            base.OnExit(e);
        }
    }
}
