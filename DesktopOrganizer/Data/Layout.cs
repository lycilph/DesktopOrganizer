using System.Collections.Generic;
using ReactiveUI;

namespace DesktopOrganizer.Data
{
    public class Layout<T>
    {
        public string Name { get; set; }
        public List<T> Items { get; set; }

        public Layout()
        {
            Items = new List<T>();
        }
    }
}
