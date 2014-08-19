using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using DesktopOrganizer.Data;
using Window = DesktopOrganizer.Data.Window;

namespace DesktopOrganizer.Utils
{
    public static class WindowManager
    {
        public static IEnumerable<Window> GetOpenWindows()
        {
            var shell_window = User32.GetShellWindow();
            var application_process_id = Process.GetCurrentProcess().Id;
            var windows = new List<Window>();

            User32.EnumWindows((wnd, param) =>
            {
                if (wnd == shell_window || !User32.IsWindowVisible(wnd)) return true;

                var length = User32.GetWindowTextLength(wnd);
                if (length == 0) return true;

                var sb = new StringBuilder(length);
                User32.GetWindowText(wnd, sb, length + 1);

                uint pid;
                User32.GetWindowThreadProcessId(wnd, out pid);
                var process = Process.GetProcessById((int)pid);
                if (process.Id == application_process_id) return true;

                var placement = new User32.WindowPlacement();
                placement.length = Marshal.SizeOf(placement);
                User32.GetWindowPlacement(wnd, ref placement);

                windows.Add(new Window
                {
                    Handle = wnd,
                    Title = sb.ToString(),
                    ProcessName = process.ProcessName,
                    ProcessPath = process.MainModule.FileName,
                    Admin = process.IsAdmin(),
                    Placement = placement
                });

                return true;
            }, 0);

            return windows;
        }

        public static IEnumerable<Program> GetPrograms(Predicate<Window> filter)
        {
            return GetOpenWindows()
                   .Where(w => !filter(w)).GroupBy(w => w.ProcessName)
                   .Select(g => new Program
                   {
                       ProcessName = g.First().ProcessName,
                       ProcessPath = g.First().ProcessPath,
                       Admin = g.First().Admin,
                       Placement = g.First().Placement,
                       Windows = g.ToList()
                   }).ToList();
        }

        public static void ApplyLayout(Layout<Program> layout)
        {
            var all_windows = GetOpenWindows().ToList();

            foreach (var program in layout.Items)
            {
                var temp = program;

                all_windows.Where(w => w.ProcessName == temp.ProcessName)
                           .Apply(w =>
                           {
                               var placement = temp.Placement;
                               User32.SetWindowPlacement(w.Handle, ref placement);
                           });
            }
        }
    }
}
