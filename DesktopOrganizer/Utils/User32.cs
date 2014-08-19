using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using DesktopOrganizer.Data;

namespace DesktopOrganizer.Utils
{
    public class User32
    {
        public delegate bool EnumWindowsProc(IntPtr wnd, int param);

        [DllImport("USER32.DLL")]
        public static extern bool EnumWindows(EnumWindowsProc enum_func, int param);

        [DllImport("USER32.DLL")]
        public static extern int GetWindowText(IntPtr wnd, StringBuilder sb, int max_count);

        [DllImport("USER32.DLL")]
        public static extern int GetWindowTextLength(IntPtr wnd);

        [DllImport("USER32.DLL")]
        public static extern bool IsWindowVisible(IntPtr wnd);

        [DllImport("USER32.DLL")]
        public static extern IntPtr GetShellWindow();

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr wnd, out uint process_id);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowPlacement(IntPtr wnd, ref WindowPlacement placement);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPlacement(IntPtr wnd, [In] ref WindowPlacement lpwndpl);

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr wnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr wnd, int id);

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
    }
}
