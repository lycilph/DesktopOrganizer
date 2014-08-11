using Caliburn.Micro.ReactiveUI;
using DesktopOrganizer.Data;
using DesktopOrganizer.Shell.ViewModels;

namespace DesktopOrganizer.Utils
{
    public class ViewModelBase : ReactiveScreen
    {
        protected readonly IShell shell;
        protected readonly ApplicationSettings application_settings;

        public ViewModelBase(IShell shell, ApplicationSettings application_settings)
        {
            this.shell = shell;
            this.application_settings = application_settings;
        }
    }
}
