using System.Diagnostics;

namespace DesktopOrganizer.Utils
{
    public static class ProcessExtensions
    {
        public static bool IsAdmin(this Process process)
        {
            return Advapi32.IsRunningAsAdministrator(process);
        }
    }
}
