using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Automation;
using System.Windows.Media.Imaging;
using Core.Data;
using Microsoft.WindowsAPICodePack.Shell;

namespace Core
{
    public static class IconManager
    {
        private static IntPtr GetDesktopWindow()
        {
            var shell_window = User32.GetShellWindow();
            var shell_default_view = User32.FindWindowEx(shell_window, IntPtr.Zero, "SHELLDLL_DefView", null);
            var sys_listview = User32.FindWindowEx(shell_default_view, IntPtr.Zero, "SysListView32", "FolderView");

            if (shell_default_view == IntPtr.Zero)
            {
                User32.EnumWindows((wnd, lParam) =>
                {
                    var sb = new StringBuilder(256);
                    User32.GetClassName(wnd, sb, sb.Capacity);

                    if (sb.ToString() == "WorkerW")
                    {
                        var child = User32.FindWindowEx(wnd, IntPtr.Zero, "SHELLDLL_DefView", null);
                        if (child != IntPtr.Zero)
                        {
                            shell_default_view = child;
                            sys_listview = User32.FindWindowEx(child, IntPtr.Zero, "SysListView32", "FolderView");
                            return false;
                        }
                    }
                    return true;
                }, 0);
            }

            return sys_listview;
        }

        private static List<string> GetIconNames(IntPtr wnd)
        {
            var el = AutomationElement.FromHandle(wnd);
            var walker = TreeWalker.ContentViewWalker;
            var names = new List<string>();
            for (var child = walker.GetFirstChild(el); child != null; child = walker.GetNextSibling(child))
            {
                names.Add(child.Current.Name);
            }
            return names;
        }

        private static int GetNumberOfIcons(IntPtr wnd)
        {
            return (int)User32.SendMessage(wnd, User32.LVM_GETITEMCOUNT, 0, IntPtr.Zero);
        }

        public static IEnumerable<Icon> GetIconsPositions()
        {
            var wnd = GetDesktopWindow();
            uint pid;
            User32.GetWindowThreadProcessId(wnd, out pid);

            var desktop_process_handle = IntPtr.Zero;
            var shared_memory_pointer = IntPtr.Zero;
            try
            {
                desktop_process_handle = Kernel32.OpenProcess(Kernel32.ProcessAccess.VmOperation | Kernel32.ProcessAccess.VmRead | Kernel32.ProcessAccess.VmWrite, false, pid);
                shared_memory_pointer = Kernel32.VirtualAllocEx(desktop_process_handle, IntPtr.Zero, 4096, Kernel32.AllocationType.Reserve | Kernel32.AllocationType.Commit, Kernel32.MemoryProtection.ReadWrite);

                var icons_count = GetNumberOfIcons(wnd);
                var names = GetIconNames(wnd);
                var icons = new List<Icon>();

                for (var index = 0; index < icons_count; index++)
                {
                    uint number_of_bytes = 0;
                    var points = new User32.IconPoint[1];

                    Kernel32.WriteProcessMemory(desktop_process_handle, shared_memory_pointer, Marshal.UnsafeAddrOfPinnedArrayElement(points, 0), Marshal.SizeOf(typeof(User32.IconPoint)), ref number_of_bytes);
                    User32.SendMessage(wnd, User32.LVM_GETITEMPOSITION, index, shared_memory_pointer);
                    Kernel32.ReadProcessMemory(desktop_process_handle, shared_memory_pointer, Marshal.UnsafeAddrOfPinnedArrayElement(points, 0), Marshal.SizeOf(typeof(User32.IconPoint)), ref number_of_bytes);

                    icons.Add(new Icon { Name = names[index], X = points[0].X, Y = points[0].Y });
                }

                return icons;
            }
            finally
            {
                if (desktop_process_handle != IntPtr.Zero)
                    Kernel32.CloseHandle(desktop_process_handle);
                if (shared_memory_pointer != IntPtr.Zero)
                    Kernel32.VirtualFreeEx(desktop_process_handle, shared_memory_pointer, 0, Kernel32.FreeType.Release);
            }
        }

        public static BitmapSource GetIcon(string item)
        {
            var obj = KnownFolders.Desktop.SingleOrDefault(i => i.GetDisplayName(DisplayNameType.Default) == item);
            return obj != null ? obj.Thumbnail.SmallBitmapSource : null;
        }
    }
}