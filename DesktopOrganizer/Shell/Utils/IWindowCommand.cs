using Caliburn.Micro;

namespace DesktopOrganizer.Shell.Utils
{
    public interface IWindowCommand : IHaveDisplayName
    {
        void Execute();
    }
}
