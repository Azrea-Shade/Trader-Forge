using System;
using System.IO;
using System.Runtime.InteropServices;
using CommunityToolkit.WinUI.Notifications; // ToastContentBuilder, ToastNotificationManagerCompat
using Windows.UI.Notifications;             // ToastNotification
using Services;

namespace Presentation
{
    /// <summary>
    /// Real Windows toast notifier for classic Win32/WPF apps.
    /// Requires a Start Menu shortcut with the same AppUserModelID.
    /// </summary>
    public sealed class WindowsToastNotifier : INotifier
    {
        private readonly string _appId;
        private readonly string _title;
        private readonly string _shortcutName;

        public WindowsToastNotifier(string appId, string title, string? shortcutName = null)
        {
            _appId = appId;
            _title = title;
            _shortcutName = shortcutName ?? "Trader Forge.lnk";
            EnsureStartMenuShortcut();
        }

        public void Notify(string title, string message)
        {
            try
            {
                // Build toast content (builder.Show(...) isn't available in 7.1.2)
                var content = new ToastContentBuilder()
                    .AddText(string.IsNullOrWhiteSpace(title) ? _title : title)
                    .AddText(message ?? string.Empty)
                    .GetToastContent();

                var xml   = content.GetXml();
                var toast = new ToastNotification(xml)
                {
                    ExpirationTime = DateTimeOffset.Now.AddMinutes(5)
                };

                ToastNotificationManagerCompat.CreateToastNotifier(_appId).Show(toast);
            }
            catch
            {
                // Swallow so notifications can't crash the app/scheduler
            }
        }

        private void EnsureStartMenuShortcut()
        {
            try
            {
                var startMenu = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
                var programs  = Path.Combine(startMenu, "Programs");
                var linkPath  = Path.Combine(programs, _shortcutName);

                if (File.Exists(linkPath)) return;

                Directory.CreateDirectory(programs);
                var exePath = Environment.ProcessPath ??
                              System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName ??
                              "TraderForge.exe";

                // Create .lnk via IShellLink and set AppUserModelID via IPropertyStore
                var shellLink = (IShellLinkW)new CShellLink();
                Marshal.ThrowExceptionForHR(shellLink.SetPath(exePath));
                Marshal.ThrowExceptionForHR(shellLink.SetArguments(""));

                var persistFile = (IPersistFile)shellLink;
                Marshal.ThrowExceptionForHR(persistFile.Save(linkPath, true));

                var propStore = (IPropertyStore)shellLink;
                var APPID_PKEY = new PROPERTYKEY(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 5); // AppUserModel.ID
                using var pv = PropVariant.FromString(_appId);
                Marshal.ThrowExceptionForHR(propStore.SetValue(ref APPID_PKEY, pv));
                Marshal.ThrowExceptionForHR(propStore.Commit());
                Marshal.ThrowExceptionForHR(persistFile.Save(linkPath, true));
            }
            catch
            {
                // Best effort; if this fails, toasts may not appear but app keeps running.
            }
        }

        #region COM interop for shell link & property store

        [ComImport, Guid("00021401-0000-0000-C000-000000000046")]
        private class CShellLink { }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
         Guid("000214F9-0000-0000-C000-000000000046")]
        private interface IShellLinkW
        {
            int GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] System.Text.StringBuilder pszFile, int cchMaxPath, IntPtr pfd, uint fFlags);
            int GetIDList(out IntPtr ppidl);
            int SetIDList(IntPtr pidl);
            int GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] System.Text.StringBuilder pszName, int cchMaxName);
            int SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
            int GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] System.Text.StringBuilder pszDir, int cchMaxPath);
            int SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
            int GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] System.Text.StringBuilder pszArgs, int cchMaxPath);
            int SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
            int GetHotkey(out short pwHotkey);
            int SetHotkey(short wHotkey);
            int GetShowCmd(out int piShowCmd);
            int SetShowCmd(int iShowCmd);
            int GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] System.Text.StringBuilder pszIconPath, int cchIconPath, out int piIcon);
            int SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
            int SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, uint dwReserved);
            int Resolve(IntPtr hwnd, uint fFlags);
            int SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
         Guid("0000010b-0000-0000-C000-000000000046")]
        private interface IPersistFile
        {
            int GetClassID(out Guid pClassID);
            int IsDirty();
            int Load([MarshalAs(UnmanagedType.LPWStr)] string pszFileName, uint dwMode);
            int Save([MarshalAs(UnmanagedType.LPWStr)] string pszFileName, bool fRemember);
            int SaveCompleted([MarshalAs(UnmanagedType.LPWStr)] string pszFileName);
            int GetCurFile([MarshalAs(UnmanagedType.LPWStr)] out string ppszFileName);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct PROPERTYKEY
        {
            public Guid fmtid;
            public uint pid;
            public PROPERTYKEY(Guid fmtid, uint pid) { this.fmtid = fmtid; this.pid = pid; }
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
         Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99")]
        private interface IPropertyStore
        {
            int GetCount(out uint cProps);
            int GetAt(uint iProp, out PROPERTYKEY pkey);
            int GetValue(ref PROPERTYKEY key, [Out] IntPtr pv);
            int SetValue(ref PROPERTYKEY key, [In] PropVariant pv);
            int Commit();
        }

        [StructLayout(LayoutKind.Explicit)]
        private sealed class PropVariant : IDisposable
        {
            [FieldOffset(0)]  private ushort vt;
            [FieldOffset(8)]  private IntPtr ptr;

            private PropVariant() { }

            public static PropVariant FromString(string value)
            {
                var pv = new PropVariant { vt = 31 }; // VT_LPWSTR
                pv.ptr = Marshal.StringToCoTaskMemUni(value);
                return pv;
            }

            public void Dispose()
            {
                if (vt == 31 && ptr != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(ptr);
                    ptr = IntPtr.Zero;
                }
                vt = 0;
            }
        }

        #endregion
    }
}
