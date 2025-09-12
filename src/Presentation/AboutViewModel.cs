using Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Presentation
{
    public partial class AboutViewModel : ObservableObject
    {
        [ObservableProperty] private string appName = "Azrea — Daily Stock & Workflow Companion";
        [ObservableProperty] private string version  = (Assembly.GetEntryAssembly()?.GetName().Version?.ToString()) ?? "1.0.0";
        [ObservableProperty] private string? logoPath;

        public AboutViewModel()
        {
            try
            {
                var baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Branding", "logos");
                if (Directory.Exists(baseDir))
                {
                    var pick = Directory.EnumerateFiles(baseDir)
                        .Where(p => p.EndsWith(".png", true, null) || p.EndsWith(".jpg", true, null) || p.EndsWith(".jpeg", true, null))
                        .OrderByDescending(p => new FileInfo(p).Length)
                        .FirstOrDefault();
                    logoPath = pick;
                }
            }
            catch { /* ignore */ }
        }
    }
}
