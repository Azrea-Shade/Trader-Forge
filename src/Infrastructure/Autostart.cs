using System;
using System.Runtime.InteropServices;

namespace Infrastructure
{
    public static class Autostart
    {
#if WINDOWS
        public static void Set(string appName, string exePath, bool enable)
        {
            try
            {
                using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Run", writable: true)
                    ?? Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                if (enable) key.SetValue(appName, $"\"{exePath}\"");
                else key.DeleteValue(appName, false);
            }
            catch { /* swallow — not critical for CI */ }
        }
#else
        public static void Set(string appName, string exePath, bool enable) { /* no-op on non-Windows */ }
#endif
    }
}
