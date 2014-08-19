using System.Collections.Generic;

namespace DesktopOrganizer.Data
{
    public class Layout<T>
    {
        public string Name { get; set; }
        public List<T> Items { get; set; }
        public Shortcut Shortcut { get; set; }

        public Layout()
        {
            Items = new List<T>();
            Shortcut = new Shortcut();
        }
    }
}
