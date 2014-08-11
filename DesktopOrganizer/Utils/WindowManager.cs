using System;
using System.Collections.Generic;
using System.Linq;
using DesktopOrganizer.Data;

namespace DesktopOrganizer.Utils
{
    public static class WindowManager
    {
        public static IEnumerable<Program> GetPrograms(Predicate<Window> filter)
        {
            return User32.GetOpenWindows(filter)
                         .GroupBy(w => w.ProcessName)
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
            //var windows = User32.GetOpenWindows(_ => false);
            //foreach (var window in windows)
            //{
            //    var program = layout.Items.FirstOrDefault(p => StringComparer.InvariantCultureIgnoreCase.Compare(p.ProcessName, window.ProcessName) == 0);
            //    if (program != null)
            //    {
            //        var placement = program.Placement;
            //        User32.SetWindowPlacement(window.Handle, ref placement);
            //    }
            //}

            //if program not running start
        }
    }
}
