using System;
using System.IO;

namespace DesktopOrganizer.Utils
{
    public class TempFile : IDisposable
    {
        public string Name { get; set; }

        public TempFile()
        {
            Name = Path.GetTempFileName();
        }

        public void Dispose()
        {
            File.Delete(Name);
        }
    }
}