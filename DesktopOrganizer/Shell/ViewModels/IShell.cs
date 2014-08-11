using DesktopOrganizer.Utils;

namespace DesktopOrganizer.Shell.ViewModels
{
    public interface IShell
    {
        void Back();
        void Show(ViewModelBase view_model);
    }
}
