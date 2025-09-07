using System;
using System.Diagnostics;
using Microsoft.Win32;
using Serilog;

namespace AzreaCompanion
{
    public class AutostartService
    {
        private const string RunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string ValueName = "AzreaCompanion";

        public void EnsureAutostart()
        {
            try
            {
                var exe = Process.GetCurrentProcess().MainModule?.FileName
                          ?? System.Reflection.Assembly.GetExecutingAssembly().Location;

                using var key = Registry.CurrentUser.OpenSubKey(RunKey, writable: true) ?? Registry.CurrentUser.CreateSubKey(RunKey, true);
                key?.SetValue(ValueName, $"\"{exe}\"");
                Log.Information("Autostart enabled at login: {Exe}", exe);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Could not set autostart (non-fatal)");
            }
        }
    }
}
