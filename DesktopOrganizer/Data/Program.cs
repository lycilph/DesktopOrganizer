using System.Collections.Generic;
using DesktopOrganizer.Utils;
using Newtonsoft.Json;

namespace DesktopOrganizer.Data
{
    public class Program
    {
        public string ProcessName { get; set; }
        public string ProcessPath { get; set; }
        public bool Admin { get; set; }
        public User32.WindowPlacement Placement { get; set; }

        // Temp data (not persisted)
        [JsonIgnore]
        public List<Window> Windows { get; set; }

        public Program()
        {
            Windows = new List<Window>();
        }
    }
}
