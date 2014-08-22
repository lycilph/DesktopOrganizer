using System;

namespace Core.Data
{
    public class Window
    {
        public IntPtr Handle { get; set; }
        public string Title { get; set; }
        public string ProcessName { get; set; }
        public string ProcessPath { get; set; }
        public bool Admin { get; set; }
        public User32.WindowPlacement Placement { get; set; }
    }
}
