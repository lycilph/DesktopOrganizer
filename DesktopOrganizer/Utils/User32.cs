using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using DesktopOrganizer.Data;

namespace DesktopOrganizer.Utils
{
    public class User32
    {
        delegate bool EnumWindowsProc(IntPtr wnd, int param);

        [DllImport("USER32.DLL")]
        static extern bool EnumWindows(EnumWindowsProc enum_func, int param);

        [DllImport("USER32.DLL")]
        static extern int GetWindowText(IntPtr wnd, StringBuilder sb, int max_count);

        [DllImport("USER32.DLL")]
        static extern int GetWindowTextLength(IntPtr wnd);

        [DllImport("USER32.DLL")]
        static extern bool IsWindowVisible(IntPtr wnd);

        [DllImport("USER32.DLL")]
        static extern IntPtr GetShellWindow();

        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr wnd, out uint process_id);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowPlacement(IntPtr wnd, ref WindowPlacement placement);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPlacement(IntPtr wnd, [In] ref WindowPlacement lpwndpl);

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct WindowPlacement
        {
            public int length;
            public int flags;
            public ShowWindowCommands show_command;
            public System.Drawing.Point min_position;
            public System.Drawing.Point max_position;
            public System.Drawing.Rectangle normal_position;
        }

        public enum ShowWindowCommands
        {
            Hide = 0,
            Normal = 1,
            Minimized = 2,
            Maximized = 3,
        }

        private static WindowPlacement GetPlacement(IntPtr wnd)
        {
            var placement = new WindowPlacement();
            placement.length = Marshal.SizeOf(placement);
            GetWindowPlacement(wnd, ref placement);
            return placement;
        }

        public static IEnumerable<Window> GetOpenWindows(Predicate<Window> filter)
        {
            var shell_window = GetShellWindow();
            var application_process_id = Process.GetCurrentProcess().Id;
            var windows = new List<Window>();

            EnumWindows(delegate(IntPtr wnd, int param)
            {
                if (wnd == shell_window || !IsWindowVisible(wnd)) return true;

                var length = GetWindowTextLength(wnd);
                if (length == 0) return true;

                var sb = new StringBuilder(length);
                GetWindowText(wnd, sb, length + 1);

                uint pid;
                GetWindowThreadProcessId(wnd, out pid);
                var process = Process.GetProcessById((int)pid);
                if (process.Id == application_process_id) return true;

                windows.Add(new Window
                {
                    Handle = wnd,
                    Title = sb.ToString(),
                    ProcessName = process.ProcessName,
                    ProcessPath = process.MainModule.FileName,
                    Admin = Advapi32.IsRunningAsAdministrator(process),
                    Placement = GetPlacement(wnd)
                });

                return true;
            }, 0);

            return windows.Where(w => !filter(w));
        }
    }
}
