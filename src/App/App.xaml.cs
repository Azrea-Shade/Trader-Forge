using System;
using System.IO;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace App
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
            Log.Logger = new LoggerConfiguration().WriteTo.File(Path.Combine(logs, "app-.log"), rollingInterval: RollingInterval.Day).CreateLogger();

            _host = Host.CreateDefaultBuilder()
                .UseSerilog()
                .ConfigureServices((ctx, services) =>
                {
                    services.AddSingleton<Infrastructure.SqliteDb>();
                    services.AddSingleton<Services.BriefingService>();
                    services.AddHttpClient();
                    services.AddSingleton<Presentation.MainViewModel>();
                    services.AddSingleton<MainWindow>();
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
