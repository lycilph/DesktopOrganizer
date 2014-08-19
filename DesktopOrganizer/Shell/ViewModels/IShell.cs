using Caliburn.Micro;
using DesktopOrganizer.Utils;

namespace DesktopOrganizer.Shell.ViewModels
{
    public interface IShell
    {
        bool Exiting { get; }

        void Back();
        void Show(ViewModelBase view_model);
    }
}
